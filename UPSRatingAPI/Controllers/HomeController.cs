using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using UPSRatingAPI.Models;
using UPSRatingAPI.UPSRateRef;

namespace UPSRatingAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly string username = WebConfigurationManager.AppSettings["UpsUsername"];
        private readonly string password = WebConfigurationManager.AppSettings["UpsPassword"];
        private readonly string accessToken = WebConfigurationManager.AppSettings["AccessToken"];

        public ActionResult Index()
        {
            //UPSSecurity upss = new UPSSecurity();
            //UPSSecurityUsernameToken upssUsrNameToken = new UPSSecurityUsernameToken();
            //UPSSecurityServiceAccessToken upssSvcAccessToken = new UPSSecurityServiceAccessToken();

            //upssUsrNameToken.Username = "Deverloper2019";
            //upssUsrNameToken.Password = "Deverloper=2019";
            //upss.UsernameToken = upssUsrNameToken;

            //upssSvcAccessToken.AccessLicenseNumber = "3D6A1DD5F39023B5";
            //upss.ServiceAccessToken = upssSvcAccessToken;

                UPSSecurity upss = new UPSSecurity();

                UPSSecurityServiceAccessToken upssSvcAccessToken = new UPSSecurityServiceAccessToken();
                upssSvcAccessToken.AccessLicenseNumber = accessToken;
                upss.ServiceAccessToken = upssSvcAccessToken;

                UPSSecurityUsernameToken upssUsrNameToken = new UPSSecurityUsernameToken();
                upssUsrNameToken.Username = username;
                upssUsrNameToken.Password = password;
                upss.UsernameToken = upssUsrNameToken;

                RateRequest rateRequest = new RateRequest();

                RequestType request = new RequestType();
                //String[] requestOption = { "Rate" };
                String[] requestOption = { "Shop" };
                request.RequestOption = requestOption;
                rateRequest.Request = request;

                ShipmentType shipment = new ShipmentType();

                ShipperType shipper = new ShipperType();
                AddressType shipperAddress = new AddressType();
                shipperAddress.City = "Roswell";
                shipperAddress.PostalCode = "30076";
                shipperAddress.StateProvinceCode = "GA";
                shipperAddress.CountryCode = "US";
                shipper.Address = shipperAddress;
                shipment.Shipper = shipper;

                ShipFromType shipFrom = new ShipFromType();
                ShipAddressType shipFromAddress = new ShipAddressType();
                shipFromAddress.City = "Roswell";
                shipFromAddress.PostalCode = "30076";
                shipFromAddress.StateProvinceCode = "GA";
                shipFromAddress.CountryCode = "US";
                shipFrom.Address = shipFromAddress;
                shipment.ShipFrom = shipFrom;

                ShipToType shipTo = new ShipToType();
                ShipToAddressType shipToAddress = new ShipToAddressType();
                shipToAddress.City = "Plam Springs";
                shipToAddress.PostalCode = "92262";
                shipToAddress.StateProvinceCode = "CA";
                shipToAddress.CountryCode = "US";
                shipTo.Address = shipToAddress;
                shipment.ShipTo = shipTo;

                CodeDescriptionType service = new CodeDescriptionType();
                //Below code uses dummy date for reference. Please udpate as required.
                service.Code = "02";
                shipment.Service = service;

                PackageType package = new PackageType();
                PackageWeightType packageWeight = new PackageWeightType();
                packageWeight.Weight = "125";
                CodeDescriptionType uom = new CodeDescriptionType();
                uom.Code = "LBS";
                uom.Description = "pounds";
                packageWeight.UnitOfMeasurement = uom;
                package.PackageWeight = packageWeight;
                CodeDescriptionType packType = new CodeDescriptionType();
                packType.Code = "02";
                package.PackagingType = packType;
                PackageType[] pkgArray = { package };
                shipment.Package = pkgArray;

                rateRequest.Shipment = shipment;

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11; //This line will ensure the latest security protocol for consuming the web service call.

                RatePortTypeClient client = new RatePortTypeClient();
                RateResponse rateResponse = client.ProcessRate(upss, rateRequest);

            RatePackage model = new RatePackage()
            {
                Response = rateResponse
            };


            return View(model);
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