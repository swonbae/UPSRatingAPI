using System;
using System.Collections.Generic;
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

            RatePackage model = new RatePackage();

            RequestRate(upss, model);
            RequestTime(upss, model);

                return View(model);
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
            addressFrom.City = "Toronto";
            addressFrom.CountryCode = "CA";
            addressFrom.PostalCode = "M1P4P5";
            //addressFrom.StateProvinceCode = "ShipFrom state province code";
            shipFrom.Address = addressFrom;
            tntRequest.ShipFrom = shipFrom;

            RequestShipToType shipTo = new RequestShipToType();
            RequestShipToAddressType addressTo = new RequestShipToAddressType();
            addressTo.City = "Toronto";
            addressTo.CountryCode = "CA";
            addressTo.PostalCode = "M1P4P5";
            //addressTo.StateProvinceCode = "ShipTo state province code";
            shipTo.Address = addressTo;
            tntRequest.ShipTo = shipTo;

            UPSTimeRef.PickupType pickup = new UPSTimeRef.PickupType();
            pickup.Date = "20190830";
            tntRequest.Pickup = pickup;

            //Below code uses dummy data for reference. Please update as required.
            UPSTimeRef.ShipmentWeightType shipmentWeight = new UPSTimeRef.ShipmentWeightType();
            shipmentWeight.Weight = "10";
            UPSTimeRef.CodeDescriptionType unitOfMeasurement = new UPSTimeRef.CodeDescriptionType();
            unitOfMeasurement.Code = "LBS";
            unitOfMeasurement.Description = "pounds";
            shipmentWeight.UnitOfMeasurement = unitOfMeasurement;
            tntRequest.ShipmentWeight = shipmentWeight;

            tntRequest.TotalPackagesInShipment = "1";
            UPSTimeRef.InvoiceLineTotalType invoiceLineTotal = new UPSTimeRef.InvoiceLineTotalType();
            invoiceLineTotal.CurrencyCode = "CAD";
            invoiceLineTotal.MonetaryValue = "10";
            tntRequest.InvoiceLineTotal = invoiceLineTotal;
            tntRequest.MaximumListSize = "1";

            //UPSSecurity upss = new UPSSecurity();
            //UPSSecurityServiceAccessToken upsSvcToken = new UPSSecurityServiceAccessToken();
            //upsSvcToken.AccessLicenseNumber = "3D6A1DD5F39023B5";
            //upss.ServiceAccessToken = upsSvcToken;
            //UPSSecurityUsernameToken upsSecUsrnameToken = new UPSSecurityUsernameToken();
            //upsSecUsrnameToken.Username = "Deverloper2019";
            //upsSecUsrnameToken.Password = "Deverloper=2019";
            //upss.UsernameToken = upsSecUsrnameToken;

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
            //String[] requestOption = { "Rate" };
            String[] requestOption = { "Shop" };
            request.RequestOption = requestOption;
            rateRequest.Request = request;

            ShipmentType shipment = new ShipmentType();

            ShipperType shipper = new ShipperType();
            AddressType shipperAddress = new AddressType();
            shipperAddress.City = "Toronto";
            shipperAddress.PostalCode = "M1P4P5";
            shipperAddress.CountryCode = "CA";
            shipper.Address = shipperAddress;
            shipment.Shipper = shipper;

            ShipFromType shipFrom = new ShipFromType();
            ShipAddressType shipFromAddress = new ShipAddressType();
            shipFromAddress.City = "Toronto";
            shipFromAddress.PostalCode = "M1P4P5";
            shipFromAddress.CountryCode = "CA";
            shipFrom.Address = shipFromAddress;
            shipment.ShipFrom = shipFrom;

            ShipToType shipTo = new ShipToType();
            ShipToAddressType shipToAddress = new ShipToAddressType();
            shipToAddress.City = "Toronto";
            shipToAddress.PostalCode = "M1P4P5";
            shipToAddress.CountryCode = "CA";
            shipTo.Address = shipToAddress;
            shipment.ShipTo = shipTo;

            UPSRateRef.CodeDescriptionType service = new UPSRateRef.CodeDescriptionType();
            //Below code uses dummy date for reference. Please udpate as required.
            service.Code = "03";
            shipment.Service = service;

            PackageType package = new PackageType();
            PackageWeightType packageWeight = new PackageWeightType();
            packageWeight.Weight = "10";
            UPSRateRef.CodeDescriptionType uom = new UPSRateRef.CodeDescriptionType();
            uom.Code = "LBS";
            uom.Description = "pounds";
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}