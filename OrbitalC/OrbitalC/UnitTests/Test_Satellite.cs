using NUnit.Framework;

namespace OrbitalC.UnitTests
{
    [TestFixture]
    public class Test_Satellite
    {
        private Satellite SAT07;
        private Satellite SAT0;
        private Satellite SAT10;

        [OneTimeSetUp]
        public void Setup()
        {
            SAT07 = new Satellite();
            SAT07.Latitude = 39.7642874925977;
            SAT07.Longitude = -75.08105558268667;
            SAT07.Altitude = 577.4354017989076;
            SAT0 = new Satellite();
            SAT0.Latitude = -45.117356836618924;
            SAT0.Longitude = 115.45816275026306;
            SAT0.Altitude = 371.3150868986114;
            SAT10 = new Satellite();
            SAT10.Latitude = -39.10177400545379;
            SAT10.Longitude = 73.19678444442621;
            SAT10.Altitude = 523.2722429168022;
        }

        [TestCase]
        public void IsVisibleToGroundPoint()
        {
            var result = this.SAT07.IsVisibleToGroundPoint(39.35902086911918, -77.70635962419638);
            Assert.IsTrue(result);
        }

        [TestCase]
        public void IsSatelliteVisible()
        {
            var result = this.SAT0.IsSatelliteVisible(this.SAT10.Latitude, this.SAT10.Longitude, this.SAT10.Altitude);
            Assert.IsTrue(result);

            result = this.SAT0.IsSatelliteVisible(this.SAT07.Latitude, this.SAT07.Longitude, this.SAT07.Altitude);
            Assert.IsFalse(result);
        }
    }
}
