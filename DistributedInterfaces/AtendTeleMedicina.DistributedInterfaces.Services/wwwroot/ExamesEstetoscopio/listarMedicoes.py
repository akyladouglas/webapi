import json
import os
import sys

import requests
from userinfo import login, senha





#nome_paciente = "Cleilton a"
#nome_paciente = "Ygor Benjamim"

############## POR REQUESTS

def getUUID(cpf):
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

        ### LIST COMPREHESSION
        # uuid = [x['attributes']['uuid'] for x in json.loads(patients.content)["data"]
        #        if x['attributes']['name'] == nome_paciente][0]

        ### NORMAL
        for x in json.loads(patients.content)["data"]:
            identifier = x['attributes']['identifier']
            if identifier  == cpf:
                uuid = x['attributes']['uuid']

        grav = f"https://app.ekodevices.com/api/v3/patients/{uuid}/search_recordings?sort_column=created_at&sort_direction=desc&page=1&per_page=12"
        arr_uuid_grav = []
        for i in range(len(json.loads(s.get(grav).content)["data"])):
            #print(json.loads(s.get(grav).content))
            uuid_grav = json.loads(s.get(grav).content)["data"][i]['attributes']['uuid']
            arr_uuid_grav.append(uuid_grav)

        print(arr_uuid_grav)


dir_ = os.getcwd()
arguments = sys.argv[1:]

try:
   # nome_paciente = ""
   # for i, nome in enumerate(arguments):
    #    if(i == len(arguments) - 1):
     #       nome_paciente += nome
     #   else:
      #      nome_paciente += nome + " "



    getUUID(arguments[0])
except Exception as e:
    print(e)
