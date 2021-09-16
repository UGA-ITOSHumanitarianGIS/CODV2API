import pandas as pd
import requests
# To read the spreadsheet file
filePath = "Location_Coordinates.xls"

''' Your input file should have two columns: Longitude and Latitude'''

file = pd.read_excel(filePath)
admin2PcodeList=[]

# To generate URL for the corresponding coordinates
for i in range(len(file)):
    long = str(file['Longitude'][i])
    lat = str(file['Latitude'][i])
    URL = "https://apps.itos.uga.edu/CODV2API/api/v1/Themes/cod-ab/Lookup/latlng?latlong="+lat+","+long+"&wkid=4326&level=3"
    response=requests.get(URL).json() # To get the content from API
    admin2PcodeList.append(response[0]['admin2Pcode']) # To get the  specific admin2Pcode

# To save the file
data=pd.DataFrame(file)
data['admin2Pcode']=admin2PcodeList
data.to_excel("output.xlsx")