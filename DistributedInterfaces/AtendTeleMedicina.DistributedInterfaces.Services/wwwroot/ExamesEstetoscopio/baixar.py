from datetime import datetime
import json
import os
import sys
import time

import requests
#from userinfo import login, senha
import mariadb
from connection import connection
import wget
from uuid import uuid4

# login = "clarissa.lima@academico.ufpb.br"
# senha = "12denovembro"

login = "clarissa.lima@academico.ufpb.br"
senha = "12denovembro"

#nome_paciente = "Cleilton a"
 
############## POR REQUESTS

def baixar(cpf, individuoId):
    with requests.Session() as s:
        # PEGAR TOKEN E COOKIES
        csrf = s.get("https://app.ekodevices.com/api/auth/csrf")

        csrf_token = json.loads(csrf.content)["csrfToken"]
        csrf_cookies = csrf.cookies

        # LOGIN
        payload = {'redirect': 'false',
                   'username': login,
                   'password': senha,
                   'csrfToken': csrf_token,
                   'callbackUrl': 'https://app.ekodevices.com/login',
                   'json': 'true'}

        credentials = s.post('https://app.ekodevices.com/api/auth/callback/credentials?', data = payload, cookies = csrf_cookies)
        
        session = s.get('https://app.ekodevices.com/api/auth/session')

        info = s.get("https://app.ekodevices.com/api/v2/user_jwt/info")

        patients = s.get("https://app.ekodevices.com/api/v3/patients?sort_column=first_name&sort_direction=asc&per_page=12&page=1&archived_state=active")

        ## LIST COMPREHESSION
        # uuid = [x['attributes']['uuid'] for x in json.loads(patients.content)["data"] 
        #        if x['attributes']['identifier'] == nome_paciente][0]

        ### NORMAL
        arr_patients = json.loads(patients.content)["data"]
        for patient in arr_patients:
            identifier = patient['attributes']['identifier']      
            if cpf == identifier:
                uuid = patient['attributes']['uuid']
            else:
                print("Não existe")

        grav = f"https://app.ekodevices.com/api/v3/patients/{uuid}/search_recordings?sort_column=created_at&sort_direction=desc&page=1&per_page=12"
        arr_uuid_grav = []
        for i in range(len(json.loads(s.get(grav).content)["data"])):
            #print(json.loads(s.get(grav).content))
            uuid_grav = json.loads(s.get(grav).content)["data"][i]['attributes']['uuid']
            arr_uuid_grav.append(uuid_grav)
        
        #print(arr_uuid_grav)

        for id in arr_uuid_grav:
            con = mariadb.connect(
                user=connection["user"],
                password=connection["password"],
                host = connection["host"],
                port=3306,
                database=connection["database"]
            )
            curWav = con.cursor()
            curWav.execute(f"SELECT * FROM ReceberExame r WHERE r.Nome LIKE '%wav_{id}%'")
            print(curWav.rowcount)
            if(curWav.rowcount == 0):
                url_audio = f'https://app.ekodevices.com/api/v3/recordings/{id}'

                r_audio = s.get(url_audio)

                arquivo_wav = json.loads(r_audio.content)
                endereco = f"wwwroot/ExamesEstetoscopio/{cpf}/"
                os.makedirs(endereco, exist_ok=True)
                arr = os.listdir(endereco)
                arquivo = id + ".wav"
                
                for arq in arr:
                    if(arq == arquivo):
                        print("Já existe")
                        
                
                    

                wget.download(
                    arquivo_wav["amazon_url_audio_filtered"], f"{endereco}{id}.wav")
                
                curWav.execute(f"SELECT * FROM ReceberExame r WHERE r.Nome LIKE '%wav_{id}%'")
                if(curWav.rowcount == 0):

                    curWav.execute(f'''
                    INSERT INTO ReceberExame 
                    (Id, Nome, IndividuoId, IndividuoCpf, TipoExameId, Formato, Url, DataDeEnvio) 
                    VALUES 
                    ("{uuid4()}", "wav_{id}", "{individuoId}", "{cpf}", "13928", ".wav", "ExamesEstetoscopio/{cpf}/{id}.wav", NOW())''')
                    con.commit()
                    curWav.close()
            else:
                print("Existe")
            
            curPdf = con.cursor()

            curPdf.execute(f"SELECT * FROM ReceberExame r WHERE r.Nome LIKE '%pdf_{id}%'")

            if(curPdf.rowcount == 0):
                ### DOWNLOAD - PDF
                arquivo = id + ".pdf"
                for arq in arr:
                    if(arq == arquivo):
                        print("Já existe")

                url_pdf = f'https://app.ekodevices.com/api/v3/recordings/{id}?show_as_pdf=true'

                r_pdf = s.get(url_pdf)

                with open(f"wwwroot//ExamesEstetoscopio//{cpf}//{id}.pdf", "wb") as f:
                    f.write(r_pdf.content)
                
                curPdf.execute(f"SELECT * FROM ReceberExame r WHERE r.Nome LIKE '%pdf_{id}%'")
                if(curPdf.rowcount == 0):

                    curPdf.execute(f'''
                    INSERT INTO ReceberExame 
                    (Id, Nome, IndividuoId, IndividuoCpf, TipoExameId, Formato, Url, DataDeEnvio) 
                    VALUES 
                    ("{uuid4()}", "pdf_{id}", "{individuoId}", "{cpf}", "13928", ".pdf", "ExamesEstetoscopio/{cpf}/{id}.pdf", NOW())''')
                    con.commit()
                    curPdf.close()
            else:
                print("Existe")
        


        print("Concluído com sucesso.")
dir_ = os.getcwd()
arguments = sys.argv[1:]

try:
   
    baixar(arguments[0], arguments[1])
    
    
except Exception as e:
    print(e)
