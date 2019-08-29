using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using UPSRatingAPI.Models;
using UPSRatingAPI.UPSRateRef;
using UPSRatingAPI.UPSTimeRef;

namespace UPSRatingAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly string username = WebConfigurationManager.AppSettings["UpsUsername"];
        private readonly string password = WebConfigurationManager.AppSettings["UpsPassword"];
        private readonly string accessToken = WebConfigurationManager.AppSettings["AccessToken"];

        private readonly string SHIPFROM_CITY = "Toronto";
        private readonly string SHIPFROM_POSTALCODE = "M1P4P5";
        private readonly string SHIPFROM_COUNTRYCODE = "CA";

        private readonly string SHIPTO_CITY = "Vancouver";
        private readonly string SHIPTO_POSTALCODE = "V5Y1V4";
        private readonly string SHIPTO_COUNTRYCODE = "CA";

        private readonly string PICKUP_DATE = "20190903";

        private readonly string WEIGHT = "10";
        private readonly string MESUREMENT = "LBS";
        //private readonly string MESUREMENT_DISCRIPTION = "pounds";

        public ActionResult Index()
        {

            UPSSecurity upss = new UPSSecurity();

            UPSSecurityServiceAccessToken upssSvcAccessToken = new UPSSecurityServiceAccessToken();
            upssSvcAccessToken.AccessLicenseNumber = accessToken;
            upss.ServiceAccessToken = upssSvcAccessToken;

            UPSSecurityUsernameToken upssUsrNameToken = new UPSSecurityUsernameToken();
            upssUsrNameToken.Username = username;
            upssUsrNameToken.Password = password;
            upss.UsernameToken = upssUsrNameToken;

            RatePackage rPackage = new RatePackage();

            RequestRate(upss, rPackage);
            RequestTime(upss, rPackage);

            return View(MatchData(rPackage).OrderBy(m => m.estimatedTime_num));
        }

        private IEnumerable<RateTimeData> MatchData(RatePackage rPackage)
        {
            List<RateTimeData> rtList = new List<RateTimeData>();

            foreach(var item in rPackage.rateResponse.RatedShipment)
            {
                var date = rPackage.timeResponse.ServiceSummary.FirstOrDefault(s => s.Service.Description.Equals(item.Service.Description)).EstimatedArrival.Arrival.Date;
                DateTime dt = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);

                rtList.Add(new RateTimeData(item.Service.Description, dt.ToString("d MMMM yyyy"), Convert.ToInt32(date), item.TotalCharges.MonetaryValue));
            }

            return rtList;
        }

        private void RequestTime(UPSSecurity upss, RatePackage rPackage)
        {
            TimeInTransitRequest tntRequest = new TimeInTransitRequest();

            UPSTimeRef.RequestType request = new UPSTimeRef.RequestType();
            String[] requestOption = { "TNT" };
            request.RequestOption = requestOption;
            tntRequest.Request = request;

            RequestShipFromType shipFrom = new RequestShipFromType();
            RequestShipFromAddressType addressFrom = new RequestShipFromAddressType();
            addressFrom.City = SHIPFROM_CITY;
            addressFrom.CountryCode = SHIPFROM_COUNTRYCODE;
            addressFrom.PostalCode = SHIPFROM_POSTALCODE;
            shipFrom.Address = addressFrom;
            tntRequest.ShipFrom = shipFrom;

            RequestShipToType shipTo = new RequestShipToType();
            RequestShipToAddressType addressTo = new RequestShipToAddressType();
            addressTo.City = SHIPTO_CITY;
            addressTo.CountryCode = SHIPTO_COUNTRYCODE;
            addressTo.PostalCode = SHIPTO_POSTALCODE;
            shipTo.Address = addressTo;
            tntRequest.ShipTo = shipTo;

            UPSTimeRef.PickupType pickup = new UPSTimeRef.PickupType();
            pickup.Date = PICKUP_DATE;
            tntRequest.Pickup = pickup;

            //Below code uses dummy data for reference. Please update as required.
            UPSTimeRef.ShipmentWeightType shipmentWeight = new UPSTimeRef.ShipmentWeightType();
            shipmentWeight.Weight = WEIGHT;
            UPSTimeRef.CodeDescriptionType unitOfMeasurement = new UPSTimeRef.CodeDescriptionType();
            unitOfMeasurement.Code = MESUREMENT;
            //unitOfMeasurement.Description = MESUREMENT_DISCRIPTION;
            shipmentWeight.UnitOfMeasurement = unitOfMeasurement;
            tntRequest.ShipmentWeight = shipmentWeight;

            tntRequest.TotalPackagesInShipment = "1";
            UPSTimeRef.InvoiceLineTotalType invoiceLineTotal = new UPSTimeRef.InvoiceLineTotalType();
            invoiceLineTotal.CurrencyCode = "CAD";
            invoiceLineTotal.MonetaryValue = "10";
            tntRequest.InvoiceLineTotal = invoiceLineTotal;
            tntRequest.MaximumListSize = "1";
            
            TimeInTransitService tntService = new TimeInTransitService();
            tntService.UPSSecurityValue = upss;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11; //This line will ensure the latest security protocol for consuming the web service call.
            Console.WriteLine(tntRequest);
            TimeInTransitResponse tntResponse = tntService.ProcessTimeInTransit(tntRequest);
            
            TransitResponseType responseItem = (TransitResponseType)tntResponse.Item;

            rPackage.timeResponse = responseItem;
        }

        private void RequestRate(UPSSecurity upss, RatePackage rPackage)
        {
            RateRequest rateRequest = new RateRequest();

            UPSRateRef.RequestType request = new UPSRateRef.RequestType();
            String[] requestOption = { "Shop" };
            request.RequestOption = requestOption;
            rateRequest.Request = request;

            ShipmentType shipment = new ShipmentType();

            ShipperType shipper = new ShipperType();
            AddressType shipperAddress = new AddressType();
            shipperAddress.City = SHIPFROM_CITY;
            shipperAddress.PostalCode = SHIPFROM_POSTALCODE;
            shipperAddress.CountryCode = SHIPFROM_COUNTRYCODE;
            shipper.Address = shipperAddress;
            shipment.Shipper = shipper;

            ShipFromType shipFrom = new ShipFromType();
            ShipAddressType shipFromAddress = new ShipAddressType();
            shipFromAddress.City = SHIPFROM_CITY; 
            shipFromAddress.PostalCode = SHIPFROM_POSTALCODE;
            shipFromAddress.CountryCode = SHIPFROM_COUNTRYCODE;
            shipFrom.Address = shipFromAddress;
            shipment.ShipFrom = shipFrom;

            ShipToType shipTo = new ShipToType();
            ShipToAddressType shipToAddress = new ShipToAddressType();
            shipToAddress.City = SHIPTO_CITY;
            shipToAddress.PostalCode = SHIPTO_POSTALCODE;
            shipToAddress.CountryCode = SHIPTO_COUNTRYCODE;
            shipTo.Address = shipToAddress;
            shipment.ShipTo = shipTo;

            PackageType package = new PackageType();
            PackageWeightType packageWeight = new PackageWeightType();
            packageWeight.Weight = WEIGHT;
            UPSRateRef.CodeDescriptionType uom = new UPSRateRef.CodeDescriptionType();
            uom.Code = MESUREMENT;
            //uom.Description = MESUREMENT_DISCRIPTION;
            packageWeight.UnitOfMeasurement = uom;
            package.PackageWeight = packageWeight;
            UPSRateRef.CodeDescriptionType packType = new UPSRateRef.CodeDescriptionType();
            packType.Code = "02";
            package.PackagingType = packType;
            PackageType[] pkgArray = { package };
            shipment.Package = pkgArray;

            rateRequest.Shipment = shipment;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11; //This line will ensure the latest security protocol for consuming the web service call.

            RatePortTypeClient client = new RatePortTypeClient();
            RateResponse rateResponse = client.ProcessRate(upss, rateRequest);


            foreach (var item in rateResponse.RatedShipment)
            {
                item.Service.Description = ServiceCode.GetName(item.Service.Code);
            }

            rPackage.rateResponse = rateResponse;
        }
        
    }
}