﻿(function () { function i() { var r = document.getElementsByTagName("head")[0], u = navigator, f = !r || !n.onloadcallback && document.readyState !== "complete" || (u.userAgent.indexOf("MSIE") !== -1 || u.vendor === "Apple Computer, Inc.") && document.readyState !== "complete" && document.body === null, i; f ? document.write('<script src="' + t + '" type="text/javascript" ><\/script>') : (i = document.createElement("script"), i.type = "text/javascript", i.language = "javascript", i.src = t, r.appendChild(i)) } var n, t; window.$MapsNamespace = window.$MapsNamespace || "Microsoft", window[$MapsNamespace] = window[$MapsNamespace] || {}, window[$MapsNamespace].Maps = window[$MapsNamespace].Maps || {}, n = { domain: "http://ecn.dev.virtualearth.net/mapcontrol/v7.0", version: "7.0.20130531151200.81", locale: "en-us", ishttpsenabled: false, onloadcallback: "__onscriptload__", tilegeneration: 1376, odmtilegeneration: 11, zoomOriginWidth: 256, defaultTileSize: 256, minMercatorZoom: 1, maxMercatorZoom: 20, maxFPS: 60, maxConcurrentTileDownloads: 12, disableVenueMaps: false, disableMicroPOI: false, disableDirections: false, disableAnalytics: false, disableSearch: false, disableTasks: false, disableMyPlaces: false, myPlacesUriFormat: "http://www.bing.com/maps/GeoCommunity.aspx", localScoutSupported: true, localScoutServiceUrlFormat: "nearby.ashx?action={action}&location={latitude},{longitude}&filters={filters}&sortby={sortby}", disableStreetside: false, streetsideGlobalMetadataUriFormat: "http://ecn.dev.virtualearth.net/REST/V1/Imagery/BlockView/StreetMetadata/ECN/{0}/{1}/{2}/{3}.js?jsonp={4}&key=AvlNiRQgHx0x9v3UNufLEfme5-g467LK_fbxxteQANuhtNoL6E9Gjhb_-Nl_FCRL", streetsideChunkMetadataUriFormat: "http://ecn.dev.virtualearth.net/REST/V1/Imagery/BlockView/ChunkMetadata/ECN/{0}/{1}/{2}/{3}.js?jsonp={4}&key=AvlNiRQgHx0x9v3UNufLEfme5-g467LK_fbxxteQANuhtNoL6E9Gjhb_-Nl_FCRL", streetsideImageryUriFormat: "http://ecn.t{1}.tiles.virtualearth.net/tiles/bvi{2}?g={0}&ir=ir0&mkt=en-us&n=f", streetsideSingleBlockLookupWithDataUriFormat: "http://dev.virtualearth.net/REST/V1/Imagery/BlockView/BlockLookupWithData/{0}/{1}/{2},{3}/{4}?jsonp={5}&key=AvlNiRQgHx0x9v3UNufLEfme5-g467LK_fbxxteQANuhtNoL6E9Gjhb_-Nl_FCRL", streetsideSingleBlockTokenLookupWithDataUriFormat: "http://dev.virtualearth.net/REST/V1/Imagery/BlockView/BlockLookupWithData/{0}/{1}/{2}?jsonp={3}&key=AvlNiRQgHx0x9v3UNufLEfme5-g467LK_fbxxteQANuhtNoL6E9Gjhb_-Nl_FCRL", streetsideCubeImageryUriFormat: "http://ecn.t{0}.tiles.virtualearth.net/tiles/hs{1}.jpg?g={2}&n=z", streetsideCubeDataLookupUriFormat: "http://dev.virtualearth.net/mapcontrol/HumanScaleServices/GetBubbles.ashx?c={0}&n={1}&s={2}&e={3}&w={4}&jsCallback={5}&key={6}", streetsideTileGeneration: "864", streetsideImageReportingUriFormat: "https://support.discoverbing.com/default.aspx?mkt={0}&productkey=bingmapprivacy&ct=eformts&(!)_SessionID=12345678&(!)_PermalinkURL={1}&(!)_Timestamp={2}&(!)_BubbleID={3}&(!)_CoordinatesofPro={4}", streetsidePermalinkUriFormat: "http://www.bing.com/maps/?v=2&cp={0}~{1}&lvl=22&sty=t~pixelOffset~{2}~streetSide~{3}~isPrivacyFocusEnabled~{4}~blockID~{5}", streetside360PrivacyReportUriFormat: "http://www.bing.com/maps/?mkt={0}&sty=x~lat~{1}~lon~{2}~alt~{3}~z~{4}~h~{5}~p~{6}~b~{7}~pid~5082&app=5082", transitTrainLineComBuyTicketsUriFormat: "http://www.thetrainline.com/buytickets/?utm_source=Microsoft&utm_medium=Maps&WT.mc_id={0}", preloadBingTheme: 0, venueMapsLandingPageEnabled: true, displayMapAppsPanel: true, isLocalEntityReportAProblemSupported: true, distanceUnitMiles: true, radixPointDecimal: true, init: function () { var r, u, n, f, t, i; for (this.protocol = this.ishttpsenabled ? "https://" : "http://", r = "onscriptload", this.onloadcallback === "__" + r + "__" && (this.onloadcallback = null), this.ishttpsenabled ? (this.onDemandDomain = "ecn-dynamic-t{subdomain}-tiles.virtualearth.net", this.staticDomain = "ecn.t{subdomain}.tiles.virtualearth.net") : (this.onDemandDomain = "ak.dynamic.t{subdomain}.tiles.virtualearth.net", this.staticDomain = "ak.t{subdomain}.tiles.virtualearth.net"), u = this.domain + "/" + this.version + "/css/en/", this.cssPath = u, n = this.domain + "/" + this.version + "/i/", f = this.domain + "/" + this.version + "/js/en-us/", this.jsPath = f, this.directionsImagePath = n + "Directions/", this.venueMapsEroServiceAppId = "BF17C84531C2B15EBE4BACA0F545AF4040188EFA", t = [["logServiceUriFormat", "{urischeme}dev.virtualearth.net/webservices/v1/LoggingService/LoggingService.svc/Log?entry=0&fmt=1&type=3&group=MapControl&name=AJAX&version={version}&mkt={culture}&auth={credentials}&jsonp=microsoftMapsNetworkCallback"], ["roadUriFormat", "{urischeme}{staticdomain}/tiles/r{quadkey}?g={generation}&mkt={culture}&lbl=l1&stl=h&shading=hill&n=z"], ["roadUnlabeledUriFormat", "{urischeme}{staticdomain}/tiles/r{quadkey}?g={generation}&mkt={culture}&lbl=l0&stl=h&shading=hill&n=z"], ["aerialUriFormat", "{urischeme}{staticdomain}/tiles/a{quadkey}.jpeg?g={generation}&n=z"], ["aerialWithLabelsUriFormat", "{urischeme}{staticdomain}/tiles/h{quadkey}.jpeg?g={generation}&mkt={culture}&n=z"], ["enhancedBirdseyeUriFormat", "{urischeme}{staticdomain}/tiles/svi{quadkey}?g={generation}&dir={dir}&n=z"], ["enhancedBirdseyeWithLabelsUriFormat", "{urischeme}{staticdomain}/tiles/cmd/svhybrid?a={quadkey}&g={generation}&dir={dir}&n=z"], ["nativeBirdseyeUriFormat", "{urischeme}{staticdomain}/tiles/o{quadkey}-{runtimeindex}-{level}-{index}?g={generation}"], ["nativeBirdseyeWithLabelsUriFormat", "{urischeme}{staticdomain}/tiles/cmd/ObliqueHybrid?a={quadkey}-{runtimeindex}-{level}-{index}&g={generation}"], ["fbUriFormat", "{urischeme}{staticdomain}/tiles/r{quadkey}?g={generation}&mkt={culture}&lbl=l1&stl=fb&shading=hill&n=z&key=AkF0mEyG789RQA6CcLimWZMzrDNF6MNSwRJOmNWb9gK_JGiwOBeMoQUoY1MFqksg"], ["fbUnlabeledUriFormat", "{urischeme}{staticdomain}/tiles/r{quadkey}?g={generation}&mkt={culture}&lbl=l0&stl=fb&shading=hill&n=z&key=AkF0mEyG789RQA6CcLimWZMzrDNF6MNSwRJOmNWb9gK_JGiwOBeMoQUoY1MFqksg"], ["weatherLightUriFormat", "{urischeme}ecn.dynamic.t{subdomain}.tiles.virtualearth.net/comp/ch/{quadkey}?mkt={culture}&it=GB&cstl=WL&og={odmgeneration}&n=z"], ["weatherDarkUriFormat", "{urischeme}ecn.dynamic.t{subdomain}.tiles.virtualearth.net/comp/ch/{quadkey}?mkt={culture}&it=GB&cstl=WD&og={odmgeneration}&n=z"], ["weatherLightLabelUriFormat", "{urischeme}ecn.dynamic.t{subdomain}.tiles.virtualearth.net/comp/ch/{quadkey}?mkt={culture}&it=Z,GF,L&cstl=WL&og={odmgeneration}&n=z"], ["weatherDarkLabelUriFormat", "{urischeme}ecn.dynamic.t{subdomain}.tiles.virtualearth.net/comp/ch/{quadkey}?mkt={culture}&it=Z,GF,L&cstl=WD&og={odmgeneration}&n=z"], ["bingMapsRESTServicesUrl", "{urischeme}dev.virtualearth.net/REST/v1/Locations"], ["imageryMetadataUrl", "{urischeme}dev.virtualearth.net/REST/V1/Imagery/Metadata/{imagerySet}?jsonp=microsoftMapsNetworkCallback&jsonso={jsono}&key={credentials}&centerPoint=47.6,-122.2&zoomLevel=1&include=ImageryProviders&culture={culture}"], ["elevationServiceUrl", "{urischeme}dev.virtualearth.net/REST/v1/Elevation/BoundingRect/{south},{west},{north},{east}/{rows}/{cols}?jsonp=microsoftMapsNetworkCallback&jsonso={jsono}&key={credentials}"], ["nativeBirdseyeMetadataUrl", "{urischeme}dev.virtualearth.net/REST/V1/Imagery/Metadata/Birdseye/{centerpoint}?jsonp=microsoftMapsNetworkCallback&jsonso={jsono}&key={credentials}&zl={zoom}&dir={heading}&dl=2"], ["venueMapsMetadataJsonpUrl", "default={urischeme}dev.virtualearth.net/REST/v1/JsonFilter/VenueMaps/data/{0}?culture={culture}&key={credentials};prod={urischeme}dev.virtualearth.net/REST/v1/JsonFilter/VenueMaps/data/{0}?culture={culture}&key={credentials};dev={urischeme}dev.virtualearth.net/REST/v1/JsonFilter/VenueMapsDev/data/{0}?culture={culture}&key={credentials};staging={urischeme}dev.virtualearth.net/REST/v1/JsonFilter/VenueMapsStaging/data/{0}?culture={culture}&key={credentials};venuemaps1={urischeme}dev.virtualearth.net/REST/v1/JsonFilter/VenueMaps1/data/{0}?culture={culture}&key={credentials};venuemaps2={urischeme}dev.virtualearth.net/REST/v1/JsonFilter/VenueMaps1/data/{0}?culture={culture}&key={credentials}"], ["venueMapsTileUrl", "default={urischeme}venuemaps.virtualearth.net/{0}/{1}/{{quadkey}}.png;prod={urischeme}venuemaps.virtualearth.net/{0}/{1}/{{quadkey}}.png;dev={urischeme}venuemapsdev.blob.core.windows.net/{0}/{1}/{{quadkey}}.png;staging={urischeme}venuemapsstaging.blob.core.windows.net/{0}/{1}/{{quadkey}}.png;venuemaps1={urischeme}venuemaps1.blob.core.windows.net/{0}/{1}/{{quadkey}}.png;venuemaps2={urischeme}venuemaps2.blob.core.windows.net/{0}/{1}/{{quadkey}}.png"], ["venueMapsNearbyUrl", "{urischeme}dev.virtualearth.net/REST/v1/VenueMaps/PointRadius/{location}/{radius}?jsonp=microsoftMapsNetworkCallback&jsonso={jsono}&output=json&key={credentials}"], ["venueMapsEroServiceUrl", "http://api.bing.net/json.aspx?AppId={appid}&Query={query}&Sources=Phonebook&Version=2.0&Market=en-us&UILanguage=en&Latitude={latitude}&Longitude={longitude}&Radius=10.0&Options=EnableHighlighting&Phonebook.Count=1&Phonebook.Offset=0&Phonebook.FileType=YP&Phonebook.SortBy=Distance&JsonType=callback&JsonCallback={jsonp}&Phonebook.LocId={ypid}"], ["hotRegionsRoadUrl", "{urischeme}{ondemanddomain}/comp/ch/{quadkey}.json?mkt={culture}&it=G,VE,BX,L,LA&shading=hill&jsonp={jsonp}&jsonso={jsonso}&og={odmgeneration}"], ["hotRegionsAerialWithLabelsUrl", "{urischeme}{ondemanddomain}/comp/ch/{quadkey}.json?mkt={culture}&it=A,G,L&shading=hill&jsonp={jsonp}&jsonso={jsonso}&og={odmgeneration}"], ["onDemandRoadUriFormat", "{urischeme}{ondemanddomain}/comp/ch/{quadkey}?mkt={culture}&it=G,VE,BX,L,LA&shading=hill&og={odmgeneration}&n=z"], ["onDemandAerialWithLabelsUriFormat", "{urischeme}{ondemanddomain}/comp/ch/{quadkey}?mkt={culture}&it=A,G,L&shading=hill&og={odmgeneration}&n=z"], ["microPOIHotRegionsUrl", "{urischeme}{ondemanddomain}/mpoi/MicroPoi/{quadkey}.json?q={query}&filter={filter}&jsonp={jsonp}&jsonso={jsonso}&output=json"], ["microPOITilesUrl", "{urischeme}{ondemanddomain}/mpoi/MicroPoi/{quadkey}?q={query}&filter={filter}"], ["streetsideCoverageMercatorUriFormat", "{urischeme}ecn.t{subdomain}.tiles.virtualearth.net/tiles/hcn{quadkey}?g={generation}&n=z"], ["streetsideCoverageBirdseyeUriFormat", "{urischeme}ecn.t{subdomain}.tiles.virtualearth.net/tiles/hcs{quadkey}?g={generation}&dir={dir}&n=z"], ["trafficUriFormat", "{urischeme}t{subdomain}.tiles.virtualearth.net/tiles/dp/content?p=tf&a={quadkey}&n=z"], ["imageryCopyrightUrl", "{urischeme}dev.virtualearth.net/REST/V1/Imagery/Copyright/{culture}/{imagerySet}/{zoom}/{minLat}/{minLon}/{maxLat}/{maxLon}?output=json&dir={heading}&jsonp=microsoftMapsNetworkCallback&jsonso={jsono}&key={credentials}"], ["directionsService", "{urischeme}dev.virtualearth.net/mapcontrol/directions.ashx?"], ["directionsMaxWaypoints", "25"], ["searchService", "{urischeme}dev.virtualearth.net/services/v1/SearchService/SearchService.asmx/Search2"], ["geocodingService", "{urischeme}dev.virtualearth.net/services/v1/geocodeservice/geocodeservice.asmx"], ["bingThemeIconUrlFormat", n + "BingTheme/pins/pin_{iconStyle}{imageryStyle}{state}.png"], ["biciLoggingService", "{urischeme}dev.virtualearth.net/mapcontrol/logging.ashx"], ["localDetailsUrl", "http://www.bing.com/local/details.aspx?lid={0}&q={1}&mkt={culture}&FORM={2}"], ["trafficIncidentsJs", "{urischeme}ecn.dev.virtualearth.net/REST/v1/Traffic/Incidents/{bounds}/?jsonp=microsoftMapsNetworkCallback&jsonso={jsono}&severity={sev}&key={credentials}"], ["restAdvancedSearchService", "{urischeme}dev.virtualearth.net/mapcontrol/search.ashx"], ["wikiDataUrl", "http://upload.maps.bing.com/WikipediaContentProviderService/WikipediaInfo.ashx?eid={0}"], ["privacyStatementLink", "http://www.microsoft.com/privacystatement/{culture}/bing/default.aspx"]], i = t.length; i--;) this[t[i][0]] = t[i][1].replace(/{ondemanddomain}/g, this.onDemandDomain).replace(/{staticdomain}/g, this.staticDomain).replace(/{urischeme}/g, this.protocol).replace(/{version}/g, this.version).replace(/{culture}/g, this.locale).replace(/{generation}/g, this.tilegeneration).replace(/{odmgeneration}/g, this.odmtilegeneration); this.defaultAerialTiltOn = !0, this.notileImageUrl = n + "notile.png", this.pushpinImageUrl = n + "poi_search.png", this.spacerImageUrl = n + "spacer.gif", this.microPOIImageUrl = n + "/MicroPOI/", this.venueMapsNumberOfMapTileServers = 4, this.venueMapsTileServerSubdomainsX = "0,2,4,6", this.venueMapsTileServerSubdomainsY = "1,3,5,7", this.logoBingMapsLink = "http://www.bing.com/maps/?v=2&cp={0}~{1}&lvl={2}&FORM=BMLOGO", this.logoBingSearchUrl = "http://www.bing.com/search?q={0}&FORM=BMSDK1", this.biciPID = "5901", this.cursorPath = this.domain + "/" + this.version + "/cursors/", this.imagePath = n, this.trafficMinZoom = 5, this.trafficMinIncidentsZoom = 8, this.trafficRefresh = 180000, this.trafficExpiry = 1800000, this.venueMapAction1Context = "Mall", this.venueMapAction2Context = "Airport", this.mapAppApp1ActionContext = "10330", this.mapAppApp2ActionContext = "10310" } }, n.init(), window[$MapsNamespace].Maps.Globals = n, t = n.jsPath + (typeof VEAPI_perflog == "undefined" ? "veapicore.js" : "veapicorePerf.js"), 0 ? n.coreJs = t : i() })()