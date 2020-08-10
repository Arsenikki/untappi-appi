# untappi-appi
It can be difficult to know what beers good for the price and if any good beers are available at all in the nearby pub! I created an application to resolve this issue, and it's available at: https://untappiappi.westeurope.cloudapp.azure.com/.

[![untappi-example.png](https://i.postimg.cc/sgckxZYX/untappi-example.png)](https://postimg.cc/188CYXYh)

## Description
The backend uses location data to fetch nearby venues and their top beers from Untappd APIs. Therefore the application gets more and more valuable beer data everytime someone visits the site! The data is managed in a PostgreSQL database in Azure and the frontend uses this data to show venue locations, which the user can click to see their top 3 beers with name, brewery, rating and country. 


### Frontend (React)
* OpenStreetMap
* React Leaflet
* Tailwind CSS


### Backend (.NET Core 3.0)
* Entity Framework Core

## To be implemented..

* Search functionality for venues and beers
* Caching
* Fetching beer images
