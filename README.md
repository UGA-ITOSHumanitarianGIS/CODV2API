# CODV2API
COD Services for Common Operational Datasets used by Humanitarian Operations -API, Documentation, Progress and Issue tracking and Resources
# Features
This repository supports community development of Common Operational Datasets maintenance and availability among humanitarian groups using data visualizations, maps and applications. The platform as of 4 June 2021 is deployed to [production](https://apps.itos.uga.edu/CODV2API) and [beta](https://beta.itos.uga.edu/). The purpose of the COD Version 2 API is to streamline features of existing COD Services, further organize and enhance metadata and docmentation for the COD portfolio and enable access among more diverse platforms and use cases as the example shows:

https://apps.itos.uga.edu/CODV2API/Content/Ex2.png

the following illustrates an api call added to Microsoft Excel.


The gistaps services available in version 1 are supplemented with additional features for version and metadata information as well as vector tiles.  The data are live and updated frequently by partners. The foll

Viewing COD mapped features in context of humanitarian operational sectors is useful for planning response, supporting analysis and data reviews.



## Operating Environment
The API uses many technologies and services and may be consumed by the Humanitarian Data Exchange [HDX](https://data.humdata.org) among other platfoms. Technologies include a docker PostGgres implementation built from Alexandru Gartner's https://github.com/OCHA-DAP/gisrestapi solution. Many other stacks inluding ESRI, GeoTools are used and creds go to so many that will be added here soon and thanks to so many for their work. ITOS is currently hosting on University of Georgia infrastructure, and a means to replicate and deploy among other humanitarians may be provided through docker, however, plans are still being formalized by the working group.

## Design and Implementation
More information coming soon.


## User Documentation
For more information on how to use the API or develop with it visit the [help](https://apps.itos.uga.edu/CODV2API/help). Check it out! There are sample calls here

•	Download a vector tile admin level for the Democratic Republic of Congo [https://apps.itos.uga.edu/CODV2API/api/v1/themes/cod-ab/locations/COD/versions/current/vectortiles/1] (https://apps.itos.uga.edu/CODV2API/api/v1/themes/cod-ab/locations/COD/versions/current/vectortiles/1)

•	Acess Metadata: https://apps.itos.uga.edu/CODV2API/api/v1/themes/cod-ab/locations/COD

•	Get Gazeteer information based on geographic coordinates: https://apps.itos.uga.edu/CODV2API/api/v1/Themes/cod-ab/Lookup/latlng?latlong=-8.89,24.98&wkid=4326&level=2


