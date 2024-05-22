import pandas as pd
import requests
import re
import os
import sys
import datetime
import xlwings as xw
# Author:      A. Russo and A. Bagodiya
# Description: Calls COD Services API to bulk generate P-Code list in a spreadsheet for geo coordinate input spreadsheet.
# Input:       Location_Coordinates.xls (The input file should have two columns: Longitude and Latitude)
# Output:      Output.xlsx
# Caveats:     COD Services API lookup uses a level and gistmaps services for the P-code ouput.
#              P-codes can be generated for coordinates in countries in the gistmaps services catalog for
#              which that specified level (see level = 2 is default below COD Services API url) exists. 
#              If a country is supported (has a service) but the level does not exist. P-codes cannot be generated.
#              If a country does not exist in the gistmaps services catalog, P-codes cannot be generated.

# To read the spreadsheet file
workbook=xw.Book.caller()

# To get the current working directory
QF = os.path.split(workbook.fullname)[0]

# To generate the log file
def log(message):
    refreshLog = QF+"\CODServicesAPIbulkPcoder.log"
    with open(refreshLog, 'a') as logMessage:
        logMessage.write('%s : %s\n' % (str(datetime.datetime.now()), str(message)))    
    return

def query_yes_no(question, default="yes"):
    #credits for the method to https://stackoverflow.com/users/103225/fmark
    """Ask a yes/no question via raw_input() and return their answer.
    "question" is a string that is presented to the user.
    "default" is the presumed answer if the user just hits <Enter>.
            It must be "yes" (the default), "no" or None (meaning
            an answer is required of the user).
    The "answer" return value is True for "yes" or False for "no".
    """
    valid = {"yes": True, "y": True, "ye": True, "no": False, "n": False}
    if default is None:
        prompt = " [y/n] "
    elif default == "yes":
        prompt = " [Y/n] "
    elif default == "no":
        prompt = " [y/N] "
    else:
        raise ValueError("invalid default answer: '%s'" % default)
    while False: 
        return
    while True:
        sys.stdout.write(question + prompt)
        choice = input().lower()
        if default is not None and choice == "":
            return valid[default]
        elif choice in valid:
            return runit(choice)
        else:
            sys.stdout.write("Please respond with 'yes' or 'no' " "(or 'y' or 'n').\n")

def main():
    query_yes_no("The input file exist here: " + QF + ", also the output.xlsx will be overwritten if it exists there. Continue?")
    print("wait")

def runit(valid):
    if valid == "no" or valid == 'n':
        return
    
    # To convert workbook into panda dataframe
    file=workbook.sheets[0].range('A1').options(pd.DataFrame, header=1, index=False, expand='table').value
    admin2PcodeList=[]

    # To generate URL for the corresponding coordinates
    for i in range(len(file)):
        long = str(file['Longitude'][i])
        lat = str(file['Latitude'][i])
        URL = "https://apps.itos.uga.edu/CODV2API/api/v1/Themes/cod-ab/Lookup/latlng?latlong="+lat+","+long+"&wkid=4326&level=2"
        response=requests.get(URL).json() # To get the content from API
        log(long + ", " + lat)
        admin2PcodeList.append(response[0]['ADM2_PCODE']) # To get the  specific admin2Pcode

    # To save the file
    file['ADM2_PCODE']=admin2PcodeList
    file.to_excel(QF+"\Output.xlsx")
    print ("Done. Check the output here: " +  QF, "CODServicesAPIbulkPcoder.log" + " and output.xlsx")
    os.system("pause")
if __name__ == "__main__":
    main()
