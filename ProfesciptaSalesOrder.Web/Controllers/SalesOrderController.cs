using Microsoft.AspNetCore.Mvc;
using ProfesciptaSalesOrder.Web.Services.SalesOrder;

namespace ProfesciptaSalesOrder.Web.Controllers;

public class SalesOrderController : Controller
{
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Create()
    {
        return View();
    }

    public ActionResult Edit(int id)
    {
        return View(id);
    }
}
