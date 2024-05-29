<template>
  <highcharts :options="graficoGlicemia"></highcharts>
</template>
<script>
  export default {
    name: 'GraficosGlicemia',
    props: {
      row: {},
    },
    data() {
      return {
        graficoGlicemia: {
          chart: {
            type: 'column'
          },
          title: {
            text: '',
          },
          subtitle: {},
          xAxis: {
            categories: ['Café da Manhã', 'Almoço', 'Janta', 'Antes de Dormir / Madrugada'],
            crosshair: true,
          },
          yAxis: {
            min: 0,
            title: {
              text: 'Medição da glicemia'
            }
          },
          tooltip: {
            valueSuffix: ' '
          },
          plotOptions: {
            column: {
              pointPadding: 0.2,
              borderWidth: 0,
              dataLabels: {
                enabled: true,
                style: {
                  textOutline: 'none'
                }
              },
            }
          },
          series: [
            {
              name: 'Antes',
              data: [],
            },
            {
              name: 'Depois',
              data: [],
              linkedTo: ':previous',
            }
          ],
          legend: {
            enabled: false,
          }
        },
      }
    },
    mounted() {
      if (this.row != undefined && this.row != {} && this.row != null) this.graficoGlicemiaMethods()
    },
    methods: {
      
      async graficoGlicemiaMethods() {

        // CAFÉ DA MANHÃ
        if (this.row.respondeuCafe != undefined && this.row.respondeuCafe == true) {
          var glicemia = this.row

          //ANTES
          var corAntes = ''
          if (glicemia.cafeAntes < 100) corAntes = '#ADD8E6'
          else if (glicemia.cafeAntes >= 100 && glicemia.cafeAntes <= 125) corAntes = '#FFD580'
          else corAntes = '#FF7F7F'

          this.graficoGlicemia.series[0].data.push({ y: glicemia.cafeAntes, color: corAntes })

          //DEPOIS
          var corDepois = ''
          if (glicemia.cafeDepois < 100) corDepois = '#ADD8E6'
          else if (glicemia.cafeDepois >= 100 && glicemia.cafeDepois <= 125) corDepois = '#FFD580'
          else corDepois = '#FF7F7F'

          this.graficoGlicemia.series[1].data.push({ y: glicemia.cafeDepois, color: corDepois })
        } else {
          this.graficoGlicemia.series[0].data.push({ y: 0, color: '#ADD8E6' })
          this.graficoGlicemia.series[1].data.push({ y: 0, color: '#ADD8E6' })
        }

        //ALMOÇO
        if (this.row.respondeuAlmoco != undefined && this.row.respondeuAlmoco == true) {
          var glicemia = this.row

          //ANTES
          var corAntes = ''
          if (glicemia.almocoAntes < 100) corAntes = '#ADD8E6'
          else if (glicemia.almocoAntes >= 100 && glicemia.almocoAntes <= 125) corAntes = '#FFD580'
          else corAntes = '#FF7F7F'

          this.graficoGlicemia.series[0].data.push({ y: glicemia.almocoAntes, color: corAntes })

          //DEPOIS
          var corDepois = ''
          if (glicemia.almocoDepois < 100) corDepois = '#ADD8E6'
          else if (glicemia.almocoDepois >= 100 && glicemia.almocoDepois <= 125) corDepois = '#FFD580'
          else corDepois = '#FF7F7F'

          this.graficoGlicemia.series[1].data.push({ y: glicemia.almocoDepois, color: corDepois })
        } else {
          this.graficoGlicemia.series[0].data.push({ y: 0, color: '#ADD8E6' })
          this.graficoGlicemia.series[1].data.push({ y: 0, color: '#ADD8E6' })
        }

        //JANTA
        if (this.row.respondeuJanta != undefined && this.row.respondeuJanta == true) {
          var glicemia = this.row

          //ANTES
          var corAntes = ''
          if (glicemia.jantaAntes < 100) corAntes = '#ADD8E6'
          else if (glicemia.jantaAntes >= 100 && glicemia.jantaAntes <= 125) corAntes = '#FFD580'
          else corAntes = '#FF7F7F'

          this.graficoGlicemia.series[0].data.push({ y: glicemia.jantaAntes, color: corAntes })

          //DEPOIS
          var corDepois = ''
          if (glicemia.jantaDepois < 100) corDepois = '#ADD8E6'
          else if (glicemia.jantaDepois >= 100 && glicemia.jantaDepois <= 125) corDepois = '#FFD580'
          else corDepois = '#FF7F7F'

          this.graficoGlicemia.series[1].data.push({ y: glicemia.jantaDepois, color: corDepois })
        } else {
          this.graficoGlicemia.series[0].data.push({ y: 0, color: '#ADD8E6' })
          this.graficoGlicemia.series[1].data.push({ y: 0, color: '#ADD8E6' })
        }

        //ANTES DE DORMIR / MADRUGADA
        if (this.row.respondeuDormirMadrugada != undefined && this.row.respondeuDormirMadrugada == true) {
          var glicemia = this.row

          //ANTES
          var corAntes = ''
          if (glicemia.antesDormirMadrugada < 100) corAntes = '#ADD8E6'
          else if (glicemia.antesDormirMadrugada >= 100 && glicemia.antesDormirMadrugada <= 125) corAntes = '#FFD580'
          else corAntes = '#FF7F7F'

          this.graficoGlicemia.series[0].data.push({ y: glicemia.antesDormirMadrugada, color: corAntes })

        } else {
          this.graficoGlicemia.series[0].data.push({ y: 0, color: '#ADD8E6' })
        }

      },
    }
  }
</script>
<style scoped>

</style>
