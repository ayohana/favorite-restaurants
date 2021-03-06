using Microsoft.AspNetCore.Mvc;
using FavoriteRestaurants.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FavoriteRestaurants.Controllers
{
  public class RestaurantsController : Controller
  {
    private readonly FavoriteRestaurantsContext _db;

    public RestaurantsController(FavoriteRestaurantsContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Restaurant> model = _db.Restaurants.Include(restaurants => restaurants.Cuisine).ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      ViewBag.CuisineId = new SelectList(_db.Cuisines, "CuisineId", "Type");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Restaurant restaurant)
    {
      _db.Restaurants.Add(restaurant);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Restaurant thisRestaurant = _db.Restaurants.FirstOrDefault(restaurant => restaurant.RestaurantId == id);
      return View(thisRestaurant);
    }

    public ActionResult Edit(int id)
    {
      var thisRestaurant = _db.Restaurants.FirstOrDefault(restaurants => restaurants.RestaurantId == id);
      ViewBag.CuisineId = new SelectList(_db.Cuisines, "CuisineId", "Type");
      return View(thisRestaurant);
    }

    [HttpPost]
    public ActionResult Edit(Restaurant restaurant)
    {
      _db.Entry(restaurant).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisRestaurant = _db.Restaurants.FirstOrDefault(restaurants => restaurants.RestaurantId == id);
      return View(thisRestaurant);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisRestaurant = _db.Restaurants.FirstOrDefault(restaurants => restaurants.RestaurantId == id);
      _db.Restaurants.Remove(thisRestaurant);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    
    public ActionResult Search()
    {
      List<Restaurant> model = _db.Restaurants.Include(restaurants => restaurants.Cuisine).OrderBy(x => x.Type).ToList();
      List<string> modelTypes = new List<string>{};
      foreach (Restaurant restaurant in model)
      {
        modelTypes.Add(restaurant.Type);
      }
      List<string> removedDuplicates = modelTypes.Distinct().ToList();
      ViewBag.RestaurantType = removedDuplicates;
      return View();
    }

    [HttpPost]
    public ActionResult SearchResults(Restaurant searchRestaurant)
    { 
          
      List<Restaurant> allModels = _db.Restaurants.Include(restaurants => restaurants.Cuisine).ToList(); 
      List<Restaurant> foundModels = allModels.ToList();

      if (searchRestaurant.Name != null)
      {
        string nameSearch = searchRestaurant.Name.ToLower();
        foundModels = foundModels.FindAll(x => x.Name.ToLower().Contains(nameSearch) == true);
      }
      if (searchRestaurant.PriceLevel != null)
      {
        foundModels = foundModels.FindAll(x => x.PriceLevel.Contains(searchRestaurant.PriceLevel) == true);
      }
      if (searchRestaurant.Rating != null)
      {
        foundModels = foundModels.FindAll(x => x.Rating.Contains(searchRestaurant.Rating) == true);
      }
      if (searchRestaurant.Type != null)
      {
        foundModels = foundModels.FindAll(x => x.Type.Contains(searchRestaurant.Type) == true);
      }
      if (searchRestaurant.Vegetarian == true)
      {
        foundModels = foundModels.FindAll(x => x.Vegetarian == true);
      }
      return View(foundModels);
    }
  }
}