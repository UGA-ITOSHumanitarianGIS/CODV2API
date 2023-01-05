import httplib2, urllib, base64, json, requests

#countries we have model and handler but no data - com
#string for numbers instead of int - grd, lca, tca
countries = ["abw","afg","alb","arg","arm","bdi","ben","bgd","blm","bmu","bol","bra","brb","btn","bwa","chl","col","cpv","cri","cub","cuw","cym","dma","dom","ecu","fji","fsm","geo","gha","glp","gmb","grd","gtm","hun","idn","irn","jam","kaz","khm","lao","lca","lka","lso","maf","mdg","mdv","mex","mhl","mli","mng","msr","mwi","mys","nam","nga","nic","npl","pak","pan","per","png","pol","pri","pse","rou","rwa","sdn","sen","sle","slv","stp","sur","svk","swz","tca","tcd","tha","tls","ton","tto","tza","uga","ukr","ury","uzb","vct","ven","vgb","vir","vnm","vut","zaf","zmb","zwe"] 
totalSum = 0
for i in range(len(countries)):
    url1 = "https://beta.itos.uga.edu/CODV2API/api/v1/themes/cod-ps/lookup/Get/0/aa/"+countries[i]
    data = urllib.request.urlopen(url1).read()
    jsonData = json.loads(data.decode("utf-8"))
    #print countries[i]
    for x in range (len(jsonData['data'])):
        totalSum += jsonData['data'][x]['T_TL']
        print (countries[i] + ", " + str(jsonData['data'][x]['T_TL']))

print ("")
print ("Total sum:")
print (totalSum)
print (len(countries))

