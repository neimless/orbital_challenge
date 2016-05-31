namespace Orbital_Calculations

open System
open NUnit.Framework

module CalculationLib = 

// Basic shared functions
    let square (x) = x * x
    let deg_to_rad (x) = x * Math.PI / 180.0
    let rad_to_deg (x) = x * (180.0 / Math.PI)
    let radian_delta (x, y) = deg_to_rad(x) - deg_to_rad(y)

    // Constant values
    let earth_radius = 6371.0
    let earth_circle = 2.0 * Math.PI * earth_radius
    let earth_segment_angle_rad (x) = deg_to_rad(360.0 * (x / earth_circle))

    // Calculate ground distance between two points
    let haversine_variable (latitude_a, latitude_b, longitude_a, longitude_b) = square(sin(radian_delta(latitude_b, latitude_a) / 2.0)) + cos(deg_to_rad(latitude_a)) * cos(deg_to_rad(latitude_b)) * square(sin(radian_delta(longitude_b, longitude_a) / 2.0))
    let ground_distance (latitude_a, latitude_b, longitude_a, longitude_b) = 2.0 * earth_radius * Math.Atan2(sqrt(haversine_variable(latitude_a, latitude_b, longitude_a, longitude_b)), sqrt(1.0 - haversine_variable(latitude_a, latitude_b, longitude_a, longitude_b)))
    
    // Calculate distance between two satellites
    // Can also be used to calculate distance from ground to satellite
    let segment_angle (latitude_a, latitude_b, longitude_a, longitude_b) = deg_to_rad(360.0 * (ground_distance(latitude_a, latitude_b, longitude_a, longitude_b) / (2.0 * Math.PI * earth_radius)))
    let satellite_distance (latitude_a, latitude_b, longitude_a, longitude_b, altitude_a, altitude_b) = sqrt(square(earth_radius + altitude_a) + square(earth_radius + altitude_b) - (2.0 * (earth_radius + altitude_a) * (earth_radius + altitude_b) * cos(segment_angle(latitude_a, latitude_b, longitude_a, longitude_b))))

    // Check if satellite is visible to a ground point
    let is_satellite_visible_to_ground (latitude_ground, latitude_sat, longitude_ground, longitude_sat, altitude_sat) : bool = ground_distance(latitude_ground, latitude_sat, longitude_ground, longitude_sat) < (rad_to_deg(acos(earth_radius / (earth_radius + altitude_sat))) / 360.0) * earth_circle 

    // Check if satellite is visible to second satellite
    let satellite_to_tangent_distance (altitude) = sqrt(square(altitude + earth_radius) - square(earth_radius))
    let satellite_angle (latitude_a, latitude_b, longitude_a, longitude_b, altitude_a, altitude_b) = asin((earth_radius + altitude_a) * (sin(earth_segment_angle_rad(ground_distance(latitude_a, latitude_b, longitude_a, longitude_b))) / satellite_distance(latitude_a, latitude_b, longitude_a, longitude_b, altitude_a, altitude_b)))
    let earth_satellite_angle (altitude) = asin(earth_radius / (earth_radius + altitude))
    let is_satellite_visible_to_satellite (latitude_a, latitude_b, longitude_a, longitude_b, altitude_a, altitude_b) : bool = 
        if satellite_distance(latitude_a, latitude_b, longitude_a, longitude_b, altitude_a, altitude_b) < satellite_to_tangent_distance(altitude_b) 
        then true
        else abs(rad_to_deg(satellite_angle(latitude_a, latitude_b, longitude_a, longitude_b, altitude_a, altitude_b))) > abs(rad_to_deg(earth_satellite_angle(altitude_b)))
    
type Satellite(name, latitude, longitude, altitude) =
    member this.Name = name
    member this.Latitude = latitude
    member this.Longitude = longitude
    member this.Altitude = altitude
    member this.IsVisibleToGround (lat, lon) = CalculationLib.is_satellite_visible_to_ground(lat, this.Latitude, lon, this.Longitude, this.Altitude)
    member this.IsVisibleToSatellite (sat:Satellite) = CalculationLib.is_satellite_visible_to_satellite(sat.Latitude, this.Latitude, sat.Longitude, this.Longitude, sat.Altitude, this.Altitude)

module Routes = 
    
    // Filter satellites that are visible to ground coordinate
    let visible_satellites (all_sats : Satellite[], latitude, longitude) = all_sats |> Array.filter (fun x -> x.IsVisibleToGround(latitude, longitude)) 

    // Filter satellites that are connected(visible) to given satellite
    let connected_satellites (all_sats : Satellite[], sat : Satellite) = all_sats |> Array.filter (fun x -> x.IsVisibleToSatellite(sat) && not(x.Name.Equals(sat.Name)))


module CalculationTests = 
    
    let SAT0 = new Satellite("SAT0",-45.117356836618924, 115.45816275026306, 371.3150868986114)
    let SAT1 = new Satellite("SAT1",-20.10052624033483, -70.44686858415544, 309.5473069628667)
    let SAT2 = new Satellite("SAT2",44.527259083924264, -35.45080284328003, 535.1606529854259)
    let SAT3 = new Satellite("SAT3",-36.49283434030968, -171.66065315272607, 339.70508901177726)
    let SAT4 = new Satellite("SAT4",-4.091925197970141, -79.87943376499916, 346.8487710241041)
    let SAT5 = new Satellite("SAT5",65.13291593919277, -127.05146239607433, 387.22721707349456)
    let SAT6 = new Satellite("SAT6",6.768512803548617, -118.994388156228, 669.7580228375812)
    let SAT7 = new Satellite("SAT7",39.7642874925977, -75.08105558268667, 577.4354017989076)
    let SAT8 = new Satellite("SAT8",27.853015057205525,50.89774329217448,476.9430043454062)
    let SAT9 = new Satellite("SAT9",-81.84663487030599,-97.37104080017033,676.656767782208)
    let SAT10 = new Satellite("SAT10",-39.10177400545379, 73.19678444442621, 523.2722429168022)
    let SAT11 = new Satellite("SAT11",-56.45194631853024,128.81208209829413,336.3804796419261)
    let SAT12 = new Satellite("SAT12",-43.989851817754314,-40.629606744519975,447.62690174241544)
    let SAT13 = new Satellite("SAT13",-64.1410099989011,-77.47937971272299,468.39263144556446)
    let SAT14 = new Satellite("SAT14",-81.34541418232814,-2.1417171446393013,662.8422114395382)
    let SAT15 = new Satellite("SAT15",68.6497740487136,66.7012107782461,595.6569335275125)
    let SAT16 = new Satellite("SAT16",2.3034044257533424,-146.26433536861217,647.9557473419012)
    let SAT17 = new Satellite("SAT17",43.992366951391716,-173.48836801988247,654.7738891688257)
    let SAT18 = new Satellite("SAT18",32.51808485112845,105.58749007174652,683.7051711134548)
    let SAT19 = new Satellite("SAT19",44.24612299405305,-161.28965111246055,490.251376673821)
    let satellite_array = [| SAT0; SAT1; SAT2; SAT3; SAT4; SAT5; SAT6; SAT7; SAT8; SAT9; SAT10; SAT11; SAT12; SAT13; SAT14; SAT15; SAT16; SAT17; SAT18; SAT19 |]
    let start_satellites = Routes.visible_satellites(satellite_array, 39.35902086911918, -77.70635962419638)
    let end_satellites = Routes.visible_satellites(satellite_array, 5.653077397552551, -75.4998834872763)

    [<Test>]
    // Check the number of starting points
    let``fsharp find starting/end points``() = 
        start_satellites |> Array.iter (fun x -> Console.WriteLine(x.Name))
        end_satellites |> Array.iter (fun x -> Console.WriteLine(x.Name))
        Assert.AreEqual(1, start_satellites.Length)
        Assert.AreEqual(1, end_satellites.Length)

    [<Test>]
    // Check number of visible satellites to SAT10
    let ``fsharp find connected satellites``() =
        Routes.connected_satellites(satellite_array, SAT10) |> Array.iter (fun x -> Console.WriteLine(x.Name))
        Assert.AreEqual(2, Routes.connected_satellites(satellite_array, SAT10).Length)

    [<Test>]
    // Starting point and SAT07
    let ``fsharp ground distance between two points``() =
        Assert.AreEqual(Math.Round(229.5111782892948, 12), Math.Round(CalculationLib.ground_distance(39.35902086911918, 39.7642874925977, -77.70635962419638, -75.08105558268667), 12))

    [<Test>]
    // SAT0 and SAT10
    let ``fsharp satellite distance``() =
        Assert.AreEqual(3710.4114052191721, CalculationLib.satellite_distance(-45.117356836618924, -39.10177400545379, 115.45816275026306, 73.19678444442621, 371.3150868986114, 523.2722429168022))

    [<Test>]
    // Starting point and SAT07
    let ``fsharp ground to satellite``() =
        Assert.AreEqual(625.20000252794716, CalculationLib.satellite_distance(39.35902086911918, 39.7642874925977, -77.70635962419638, -75.08105558268667, 0.0, 577.4354017989076))

    [<Test>]
    // SAT07 should be visible to startingpoint
    let ``fsharp SAT07 is visible to startingpoint``() =
        Assert.IsTrue(CalculationLib.is_satellite_visible_to_ground(39.35902086911918, 39.7642874925977, -77.70635962419638, -75.08105558268667, 577.4354017989076))        
        Assert.IsTrue(SAT7.IsVisibleToGround(39.35902086911918, -77.70635962419638))

    [<Test>]
    // SAT0 should be visible to SAT10
    let ``fsharp SAT0 is visible to SAT10``() =        
        Assert.IsTrue(CalculationLib.is_satellite_visible_to_satellite(-45.117356836618924, -39.10177400545379, 115.45816275026306, 73.19678444442621, 371.3150868986114, 523.2722429168022))
        Assert.IsTrue(SAT0.IsVisibleToSatellite(SAT10))

    [<Test>]
    // SAT0 should be visible to SAT7
    let ``fsharp SAT0 is not visible to SAT7``() =
        Assert.IsFalse(CalculationLib.is_satellite_visible_to_satellite(-45.117356836618924, 39.7642874925977, 115.45816275026306, -75.08105558268667, 371.3150868986114, 577.4354017989076))