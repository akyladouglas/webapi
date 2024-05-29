<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      
      <el-row>
        <el-col :span="14">
          <!-- <h2 class="box-card--h2">{{$route.meta.title}}</h2> -->
          <h2 class="box-card--h2">Mapeamento</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <FiltroMapeamento :loading="loading.individuos" :params="params" :onMap="true" @emitHeatMap="onEmitFromMap" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row> 

      <el-row v-show="!isDisabled">
        <el-col :span="24">
          <div id="map" ref="map">
          </div>
        </el-col>
        <el-col :span="24">
          <small>Total: {{ this.api.individuos.length }}</small>
        </el-col>
      </el-row>
    </el-card>

  </el-col>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroMapeamento from '../../components/shared/FiltroMapeamento'
  import jQuery from 'jquery'
  export default {
    mixins: [Utils],
    name: 'IndividuoMapeamento',
    components: { FiltroMapeamento },
    data () {
      return {
        showHeatMap: false,
        showEstabelecimentos: false,
        showMarkers: false,
        isDisabled: true,
        map: null,
        heatmap: null,
        markers: [],
        api: {
          individuos: [],
          estabelecimentos: []
        },
        loading: {
          individuos: false,
          estabelecimentos: false
        },
        enums: {
          coresStatus: _enums.coresStatus
        },
        params: {
          page: 1,
          pageSize: 10,
          corStatusIn: [1, 2, 3, 4, 5],
          sort: 'i.CorStatus DESC, i.NomeCompleto ASC',
          total: 0,
          estabelecimentos: {
            skip: 1,
            take: 999,
            ativo: true,
            sort: '+Nome',
            total: 0,
            ufAbreviado: null,
            cidadeIBGE: null
          }
        }
      }
    },
    watch: {
      showMarkers: function (newVal, oldVal) {
        this.toggleMarkers(newVal)
      },
      showHeatMap: function (newVal, oldVal) {
        this.toggleHeatmap(newVal)
      },
      showEstabelecimentos: function (newVal, oldVal) {
        this.toggleEstabelecimentos(newVal)
      }
    },
    created () {
      this.showMarkers = true
      this.showHeatMap = false
      this.showEstabelecimentos = false
    },
    async mounted () {
      this.initMap({ lat: -9.6512579, lng: -45.0679933, zoom: 6 })
    },
    methods: {
      async onEmitFromFiltro (val) {
        this.markers = []
        if (!val.params.uf) {

          this.$swal({
            title: "Atenção!",
            text: 'O filtro de estado é obrigatório!',
            icon: 'warning',
          })

          return
        }
        this.params = {
          ...this.params,
          ...val.params
        }
        this.params.estabelecimentos = {
          ...this.params.estabelecimentos,
          ufAbreviado: val.params.uf ? val.params.uf : null,
          cidadeIBGE: val.params.cidadeIBGE ? val.params.cidadeIBGE : null
        }
        this.listando = true
        await this.getIndividuosMapeamento()
        await this.getEstabelecimentos()
        this.toggleMarkers(this.showMarkers)
        this.toggleHeatmap(this.showHeatMap)
        // if (this.showEstabelecimentos) {
        //   this.toggleEstabelecimentos(true)
        // } else {
        //   this.toggleEstabelecimentos(false)
        // }
      },
      initMap (val) {
        this.map = new window.google.maps.Map(this.$refs['map'], {
          center: { lat: val.lat, lng: val.lng },
          zoom: val.zoom
        })
        this.heatmap = new window.google.maps.visualization.HeatmapLayer({
          data: [],
          map: this.map
        })
      },
      getMap (callback) {
        let vm = this
        function checkForMap () {
          if (vm.map) callback(vm.map)
          else setTimeout(checkForMap, 200)
        }
        checkForMap()
      },
      async onEmitFromMap (val) {
        this.showHeatMap = val.showHeatMap
        this.showMarkers = val.showMarkers
        this.showEstabelecimentos = val.showEstabelecimentos
      },
      async getIndividuosMapeamento () {
        this.loading.individuos = true
        let { data, status } = await _api.individuos.getMapeamento(this.params)
        this.api.individuos = (status === 200) ? data : []
        this.params.total = (status === 200) ? this.api.individuos.length : 0
        this.loading.individuos = false
        if (data.length === 0) {
          this.initMap({ lat: -9.6512579, lng: -45.0679933, zoom: 6 })

          this.$swal({
            title: "Atenção!",
            text: 'Nenhum resultado encontrado com base nos filtros utilizados!',
            icon: 'warning',
          })

          return
        }
        jQuery('html, body').animate({
          scrollTop: jQuery(window).scrollTop() + 520
        }, 1000)
        this.isDisabled = false
        await this.setMarkers()
      },
      async getEstabelecimentos () {
        this.loading.estabelecimentos = true
        let { data, status } = await _api.estabelecimentos.get(this.params.estabelecimentos)
        this.api.estabelecimentos = (status === 200) ? data : []
        this.params.total = (status === 200) ? this.api.estabelecimentos.length : 0
        this.loading.estabelecimentos = false
        if (!data && this.showEstabelecimentos) {

          this.$swal({
            title: "Atenção!",
            text: 'Nenhum estabelecimento encontrado!',
            icon: 'warning',
          })

          return
        }
        await this.setMarkersEstabelecimentos()
        if (!this.showEstabelecimentos) this.setMapOnAllEstabelecimentos(null)
      },
      getPoints () {
        let points = []
        this.api.individuos.forEach(i => {
          points.push(new window.google.maps.LatLng(i.latitudeIndividuo, i.longitudeIndividuo))
        })
        return points
      },
      toggleHeatmap (val) {
        if (val) {
          this.heatmap = new window.google.maps.visualization.HeatmapLayer({
            data: this.getPoints(),
            map: this.map
          })
          this.heatmap.set('radius', this.heatmap.get('radius') ? null : 50)
        } else {
          this.heatmap.setMap(null)
        }
      },
      toggleMarkers (val) {
        if (val) {
          this.setMapOnAll(this.map)
        } else {
          this.setMapOnAll(null)
        }
      },
      toggleEstabelecimentos (val) {
        if (val) {
          if (this.api.estabelecimentos.length === 0) {
            this.$swal({
              title: "Atenção!",
              text: 'Nenhum estabelecimento encontrado!',
              icon: 'warning',
            })
          }
          this.setMapOnAllEstabelecimentos(this.map)
        } else {
          this.setMapOnAllEstabelecimentos(null)
        }
      },
      setMapOnAll (map) {
        this.markers.forEach(m => {
          if (m.tipo === 'individuo') {
            m.setMap(map)
          }
        })
      },
      setMapOnAllEstabelecimentos (map) {
        this.markers.forEach(m => {
          if (m.tipo === 'estabelecimento') {
            m.setMap(map)
          }
        })
      },
      async setMarkers () {
        this.isDisabled = false
        let mCenter = this.api.individuos[0]
        this.initMap({ lat: mCenter.latitudeIndividuo, lng: mCenter.longitudeIndividuo, zoom: 11 })
        var infowindow = new window.google.maps.InfoWindow()
        this.api.individuos.forEach(i => {
          let icone = new window.google.maps.MarkerImage(`https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|${this.getCor(i.corStatus).replace('#', '')}`);
          let infoMarcador = `
            <h6>Dados do Cidadão</h6>
            Nome: ${i.nomeCompleto} <br />
            Cpf: ${i.cpf} <br />
            Idade: ${this.moment().diff(i.dataNascimento, 'years')} anos <br />
            Data Cadastro: ${this.moment(i.dataCadastro).format('DD/MM/YYYY')} <br />
            Data 1º Sintoma: ${i.hasOwnProperty('dataInicioSintomas') ? this.moment(i.dataInicioSintomas).format('DD/MM/YYYY') : 'Não Houve'} <br />
            Dias em Quarentena: ${i.hasOwnProperty('dataIsolamento') ? this.moment().diff(i.dataIsolamento, 'days') : 'Não Houve'} <br />
            Data Encerramento: ${i.hasOwnProperty('dataEncerramento') ? this.moment(i.dataEncerramento).format('DD/MM/YYYY') : 'Não Houve'} <br />
          `
          let marker = new window.google.maps.Marker({
            position: { lat: i.latitudeIndividuo, lng: i.longitudeIndividuo },
            map: this.map,
            icon: icone,
            tipo: 'individuo'
          })
          if (this.mxHasAccess('Administrador', 'Atendente', 'Estudante', 'GestorMunicipioAmpliado')) {
            marker.addListener('click', function () {
              infowindow.setContent(infoMarcador)
              infowindow.open(this.map, marker)
              this.map.panTo(this.getPosition())
            })
          }
          this.markers.push(marker)
        })
      },
      async setMarkersEstabelecimentos () {
        var infowindow = new window.google.maps.InfoWindow()
        this.api.estabelecimentos.forEach(i => {
          let icone = new window.google.maps.MarkerImage('https://chart.apis.google.com/chart?chst=d_bubble_icon_text_small&chld=medical|bb||006400')
          let infoMarcador = `
            <h6>Dados do Estabelecimento</h6>
            Nome: ${i.nome} <br />
            CNES: ${i.cnes} <br />
            Cód. da Cidade: ${i.cidadeIBGE} <br />
            Telefone: ${i.hasOwnProperty('telefone') ? i.telefone : 'Não Informado'} <br />
            Endereco: ${i.logradouro}, ${i.numero} <br />
            Bairro: ${i.bairro} <br />
          `
          let marker = new window.google.maps.Marker({
            position: { lat: parseFloat(i.latitude), lng: parseFloat(i.longitude) },
            map: this.map,
            icon: icone,
            tipo: 'estabelecimento'
          })
          marker.addListener('click', function () {
            infowindow.setContent(infoMarcador)
            infowindow.open(this.map, marker)
            this.map.panTo(this.getPosition())
          })
          this.markers.push(marker)
        })
      },
      getCor (corStatus) {
        return this._.find(this.enums.coresStatus, { value: corStatus }).cor
      }
    }
  }
</script>

<style scoped>
 #map {
   height: 600px;
   background: gray;
 }
</style>
