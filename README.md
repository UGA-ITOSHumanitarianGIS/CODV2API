# CODV2API
[production](https://apps.itos.uga.edu/CODV2API)
COD Services for Common Operational Datasets used by Humanitarian Operations -API, Documentation, Resources, Progress and Issue tracking.
# Features
This repository supports community development of Common Operational Datasets (COD) maintenance and availability among humanitarian groups using data visualizations, maps and applications. The platform as of 4 June 2021 is deployed to [production](https://apps.itos.uga.edu/CODV2API) and [beta](https://beta.itos.uga.edu/CODV2API). The purpose of the COD Version 2 API is to streamline features of existing COD Services, further organize and enhance metadata and docmentation for the COD portfolio and enable access among more diverse platforms and use cases as the example shows:

![alt text](https://apps.itos.uga.edu/CODV2API/Content/Ex2.png "COD API and Clients") 

Check out COD Services API live call results in Microsoft Excel:

![alt text](https://apps.itos.uga.edu/CODV2API/Content/excel-ex.png "COD API Excel") 

And -- the purpose of [CODs](https://cod.unocha.org) through the stewardship at the United Nations Office for the Coordination of Humanitarian Affairs (UN OCHA): Integrating COD datasets featuring mapping capability in context of humanitarian operational sectors is useful for planning disaster response, supporting analysis and data reviews...and monitoring outcomes.

The [gismaps](https://gistmaps.itos.uga.edu/arcgis/rest/services/cod-external) ArcGIS services available in version 1 that suppored humanitarian COD uses are supplemented with additional features for version and metadata information as well as vector tiles in this project. The data are live and updated frequently by partners. A data model and more usage examples may be found in the project wiki.

Another feature to this resource is to promote widespread access to country and sub-country location level resources and make standardization, collaboration, data transparency and data literacy an integrated process to work already in place for humanitarians. The services from gistmaps are not edge-matched (as for regional and global datasets). So these require additonal steps for cartographic consumption at those levels most likely. UN SALB, Fieldmaps.io and other data collaborators are working at these. ITOS has provided regional datasets in the past based on CODs, however these are not in sync with a version or the latest country data and this syncing or maintenance effort for the regional and global cartographic project is outside the scope of this project. This project does provide a more seamless global access to these resources, where, for example, the API look up, such as passing a location P-Code or geographic coordinates to the API (see Excel results above which include such a look up) do not require the country parameter or input for example as the locations are returned (a product of the streamlining).


## Operating Environment
The API uses many technologies and services and may be consumed by the Humanitarian Data Exchange [HDX](https://data.humdata.org) among other platfoms. Technologies include a docker Postgres implementation built from Alexandru Gartner's https://github.com/OCHA-DAP/gisrestapi solution. Many other stacks inluding ESRI, GeoTools are used and creds go to so many that will be added here soon and thanks to so many for their work. ITOS is currently hosting on University of Georgia infrastructure, and a means to replicate and deploy among other humanitarians may be provided through docker, however, plans are still being formalized by the working group.

## Design and Implementation
More information coming soon.


## User Documentation
For more information on how to use the API or develop with it visit the [help](https://apps.itos.uga.edu/CODV2API/help). Check it out! There are sample calls here

•	Build applications and visualizations using hosted vector tile cod-ab. The help documentation showcases a Leaflet JSFiddle for Haiti (https://apps.itos.uga.edu/CODV2API/Help)

•	Download a vector tile admin level for the Democratic Republic of Congo (https://apps.itos.uga.edu/CODV2API/api/v1/themes/cod-ab/locations/COD/versions/current/vectortiles/1)

•	Access Metadata: https://apps.itos.uga.edu/CODV2API/api/v1/themes/cod-ab/locations/COD

•	Get Gazeteer information based on geographic coordinates: https://apps.itos.uga.edu/CODV2API/api/v1/Themes/cod-ab/Lookup/latlng?latlong=-8.89,24.98&wkid=4326&level=2

## Contributions

• Funded by the United States Agency for International Development Bureau of Disaster Assistance

• United Nations Office for the Coordination of Humanitarian Affairs

• United Nations and NGO partners.

• University of Georgia, Carl Vinson Institute of Government, Information Technology Outreach Services Division



