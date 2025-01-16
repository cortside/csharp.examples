# WeatherForecast.WebApi

This is an example webapi project that shows:

* How to have 2 different endpoints that differ only by the argument type
    * /api/v1/forecasts/{{intId:int))
    * /api/v1/forecasts/{{guidId:guid}}


The keys points to note are:

* the argument types must be different
    * i.e. int and guid
* specify the parsed type in the method attribute
    * `[HttpGet("{intId:int}")]`
    * `[HttpGet("{guidId:guid}")]`
* the names of the arguments must be different
    * swagger uses the argument name to generate the unique paths
