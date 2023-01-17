import http.client, urllib.request, urllib.parse, urllib.error, base64, json, socket

def unique(list1):
  
    # initialize a null list
    unique_list = []
  
    # traverse for all elements
    for x in list1:
        # check if exists in unique_list or not
        if x not in unique_list:
            unique_list.append(x)
    # print list
    for x in unique_list:
        print (x)

countries = [["afg", "af", 1],["alb", "al", 1],["arg", "ar", 1],["arm", "am", 1],["bdi", "bi", 2],["ben", "bj", 2],["bgd", "bd", 3],["blm", "bl", 1],["bmu", "bm", 0],["bol", "bo", 1],["bra", "br", 1],["brb", "bb", 1],["btn", "bt", 1],["bwa", "bw", 3],
             ["chl", "cl", 3],["col", "co", 2],["com", "km", 1],["cpv", "cv", 1],["cri", "cr", 3],["cub", "cu", 2],["cym", "ky", 1],["dom", "do", 3],["ecu", "ec", 1],["fji", "fj", 3],["fsm", "fm", 2],["geo", "ge", 2],["gha", "gh", 2],["glp", "gp", 2],
             ["gmb", "gm", 1],["grd", "gd", 1],["gtm", "gt", 2],["hun", "hu", 1],["irn", "ir", 2],["jam", "jm", 1],["kaz", "kz", 1],["khm", "kh", 3],["lao", "la", 2],["lca", "lc", 1],["lka", "lk", 2],["lso", "ls", 1],["maf", "mf", 1],["mdg", "mg", 4],
             ["mex", "mx", 2],["mhl", "mh", 2],["mli", "ml", 3],["mng", "mn", 2],["msr", "ms", 1],["mwi", "mw", 2],["mys", "my", 1],["nam", "na", 2],["nga", "ng", 2],["nic", "ni", 2],["npl", "np", 2],["pak", "pk", 3],["pan", "pa", 1],["per", "pe", 1],
             ["png", "pg", 3],["pol", "pl", 1],["pri", "pr", 1],["pse", "ps", 1],["rou", "ro", 2],["rwa", "rw", 3],["sen", "sn", 3],["sle", "sl", 3],["slv", "sv", 2],["stp", "st", 2],["sur", "sr", 2],["svk", "sk", 2],["swz", "sz", 1],["tca", "tc", 1],
             ["tcd", "td", 2],["tha", "th", 2],["ton", "to", 3],["tto", "tt", 1],["tza", "tz", 2],["uga", "ug", 2],["ukr", "ua", 1],["ury", "uy", 1],["uzb", "uz", 1],["vct", "vc", 2],["vgb", "vg", 1],["vir", "vi", 2],["vnm", "vn", 1],["vut", "vu", 2],
             ["zmb", "zm", 1],["zwe", "zw", 1]]
finalList = []
trueList = []
falseList = []
i=0
iterator = len(countries)
iterator = iterator - 1
while i < iterator:
    iso2 = countries[i][1]
    iso3 = countries[i][0]
    admLvl = countries[i][2]
    statusString = ""
    if iso3 == "com":
        i += 1
        continue
    url1 = "https://beta.itos.uga.edu/CODV2API/api/v1/themes/cod-ps/lookup/Get/"+str(admLvl)+"/aa/"+iso3
    url2 = "https://beta.itos.uga.edu/CODV2API/api/v1/themes/cod-ab/lookup/"+str(admLvl)+"/"+iso2
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  
    s.settimeout(120)
    
    data = urllib.request.urlopen(url1, timeout=30).read()
    jsonData = json.loads(data.decode("utf-8"))

    data2 = urllib.request.urlopen(url2, timeout=30).read()
    jsonData2 = json.loads(data2.decode("utf-8"))
    if len(jsonData2) == 1:
        statusString = "No Cod AB available"
        finalList.append(iso3)
    else:
        if len(jsonData['data']) == len(jsonData2):
            trueList.append(iso3)
        else:
            falseList.append([iso3,iso2,admLvl])
    
    while admLvl > 1:
        admLvl = admLvl - 1
        url1 = "https://beta.itos.uga.edu/CODV2API/api/v1/themes/cod-ps/lookup/Get/"+str(admLvl)+"/aa/"+iso3
        url2 = "https://beta.itos.uga.edu/CODV2API/api/v1/themes/cod-ab/lookup/"+str(admLvl)+"/"+iso2

        data = urllib.request.urlopen(url1, timeout=30).read()
        jsonData = json.loads(data.decode("utf-8"))

        data2 = urllib.request.urlopen(url2, timeout=30).read()
        jsonData2 = json.loads(data2.decode("utf-8"))
        if len(jsonData2) == 1:
            statusString = "No Cod AB available"
            finalList.append(iso3)
        else:
            if len(jsonData['data']) == len(jsonData2):
                trueList.append(iso3)
            else:
                falseList.append([iso3,iso2,admLvl])
        
    print((iso3+" finished"))
    i += 1
print("Countries with matching AB and PS")
unique(trueList)
print("\n")
print("Countries with different AB and PS")
unique(falseList)
print("\n")
print("Countries without a COD AB")
unique(finalList)

