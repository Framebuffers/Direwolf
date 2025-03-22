using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct ProjectSiteIntrospection(Document document)
    {
        public string placeName
        {
            get
            {
                try
                {
                    return document.SiteLocation.PlaceName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public double elevation
        {
            get
            {
                try
                {

                    return document.SiteLocation.Elevation;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public double latitude
        {
            get
            {
                try
                {

                    return document.SiteLocation.Latitude;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public double longitude
        {
            get
            {
                try
                {

                    return document.SiteLocation.Longitude;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public double timeZone
        {
            get
            {
                try
                {

                return document.SiteLocation.TimeZone;
                }
                catch
                {
                    return -1;
                }
            }
        }
        public string geoCoordinateSystemId
        {
            get
            {
                try
                {

                    return document.SiteLocation.GeoCoordinateSystemId;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        public string geoCoordinateSystemDefinition
        {
            get
            {
                try
                {

                    return document.SiteLocation.GeoCoordinateSystemDefinition;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }

}





