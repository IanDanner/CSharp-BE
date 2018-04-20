using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using C_SharpBelt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace C_SharpBelt.Controllers
{
    public class HomeController : Controller
    {
        private CBeltContext _context;
 
        public HomeController(CBeltContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Home")]
        public IActionResult Index()
        {
            int? loggedId = HttpContext.Session.GetInt32("loggedId");
            if(loggedId == null){
                return RedirectToAction("Index", "LoginReg");
            }
            User logged = _context.users.Include(make=>make.CreatedActivities).Include(go=>go.Participating).ThenInclude(where=>where.activities).Where(we => we.id == loggedId).SingleOrDefault();
            List<Activitie> activities = _context.activities.Include(people=>people.Participants).ThenInclude(who=>who.users).OrderBy(time=>time.startDate).ToList();

            foreach(Activitie wed in activities){
                DateTime now = DateTime.Now;
                if(wed.startDate < now){
                    int actNum = wed.id;
                    DeleteActivity(actNum);          
                    return RedirectToAction("Index");
                }
            }
            
            activities = _context.activities.Include(people=>people.Participants).ThenInclude(who=>who.users).OrderBy(time=>time.startDate).ToList();

            ViewBag.Activities = activities;
            ViewBag.Logged = logged;
            return View();
        }

        [HttpGet]
        [Route("activity/{activityId}")]
        public IActionResult ActivityInfo(int activityId)
        {            
            int? loggedId = HttpContext.Session.GetInt32("loggedId");
            if(loggedId == null){
                return RedirectToAction("Index", "LoginReg");
            }
            User logged = _context.users.Include(make=>make.CreatedActivities).Include(go=>go.Participating).ThenInclude(where=>where.activities).Where(we => we.id == loggedId).SingleOrDefault();
            
            Activitie activity = _context.activities.Include(people=>people.Participants).ThenInclude(who=>who.users).SingleOrDefault(where=>where.id == activityId);
            
            ViewBag.Activity = activity;
            ViewBag.Logged = logged;
            
            return View();
        }

        [HttpGet]
        [Route("new_activity")]
        public IActionResult NewActivity()
        {   
            int? loggedId = HttpContext.Session.GetInt32("loggedId");
            if(loggedId == null){
                return RedirectToAction("Index", "LoginReg");
            }

            return View();
        }

        [HttpGet]
        [HttpPost]
        [Route("create_activity")]
        public IActionResult CreateActivity(ActivitieCreate info, string durationTime)
        {
            int? loggedId = HttpContext.Session.GetInt32("loggedId");
            if(loggedId == null){
                return RedirectToAction("Index", "LoginReg");
            }
            
            if(ModelState.IsValid)
            {
                if(info.startDate < DateTime.Now){
                    TempData["Error"] = "Start Date must be in the Future";          
                    return RedirectToAction("NewActivity", info);
                }

                User logged = _context.users.SingleOrDefault(we => we.id == loggedId);             

                Activitie newAct = new Activitie
                {
                    title = info.title,
                    startDate = info.startDate,
                    duration = info.duration.ToString()+" "+durationTime,
                    description = info.description,
                    userId = logged.id,
                    user = logged
                };

                List<Participant> participants = _context.participants.Include(stuff=>stuff.activities).Where(we => we.usersId == loggedId).ToList();

                foreach(Participant part in participants){
                    if(part.activities.startDate > newAct.startDate){
                        if(!(part.activities.startDate > AddDuration(newAct.startDate , newAct.duration))){
                            TempData["Error"]= $"{newAct.title} conflicts with {part.activities.title} in which you are participating";
                            return View("NewActivity", info);
                        }
                    }
                    if(part.activities.startDate <= newAct.startDate){
                        if(!(AddDuration(part.activities.startDate,part.activities.duration)<=newAct.startDate)){
                            TempData["Error"]= $"{newAct.title} conflicts with {part.activities.title} in which you are participating";
                            return View("NewActivity", info);
                        }
                    }
                }

                _context.activities.Add(newAct);
                _context.SaveChanges();
                
                Participant newPart = new Participant
                {
                    activitiesId = newAct.id,
                    activities = newAct,
                    usersId = logged.id,
                    users = logged,
                };
                _context.participants.Add(newPart);
                _context.SaveChanges();
                return RedirectToAction("ActivityInfo", new { activityId = newAct.id});
            }
            return View("NewActivity", info);
        }

        [HttpGet]
        [Route("delete_activity/{activityId}")]
        public IActionResult DeleteActivity(int activityId)
        {   
            int? loggedId = HttpContext.Session.GetInt32("loggedId");
            if(loggedId == null){
                return RedirectToAction("Index", "LoginReg");
            }

            Activitie activity = _context.activities.SingleOrDefault(del=>del.id == activityId);

            List<Participant> participants = _context.participants.Where(we => we.activitiesId == activityId).ToList();
            foreach(Participant part in participants){
                _context.participants.Remove(part);
                _context.SaveChanges();
            }

            _context.activities.Remove(activity);
            _context.SaveChanges();            

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("join_activity/{activityId}")]
        public IActionResult JoinActivity(int activityId)
        {   
            int? loggedId = HttpContext.Session.GetInt32("loggedId");
            if(loggedId == null){
                return RedirectToAction("Index", "LoginReg");
            }

            User logged = _context.users.SingleOrDefault(we => we.id == loggedId);
            Activitie activity = _context.activities.SingleOrDefault(del=>del.id == activityId);
            List<Participant> participants = _context.participants.Include(stuff=>stuff.activities).Where(we => we.usersId == loggedId).ToList();

            foreach(Participant part in participants){
                if(part.activities.startDate > activity.startDate){
                    if(!(part.activities.startDate > AddDuration(activity.startDate , activity.duration))){
                        TempData["Error"]= $"{activity.title} conflicts with {part.activities.title}";
                        return RedirectToAction("Index");
                    }
                }
                if(part.activities.startDate <= activity.startDate){
                    if(!(AddDuration(part.activities.startDate,part.activities.duration)<=activity.startDate)){
                        TempData["Error"]= $"{activity.title} start conflicts with {part.activities.title}";
                        return RedirectToAction("Index");
                    }
                }
            }

            Participant imPart = new Participant
            {
                activitiesId = activity.id,
                activities = activity,
                usersId = logged.id,
                users = logged,
            };
            _context.participants.Add(imPart);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("leave_activity/{activityId}")]
        public IActionResult LeaveActivitie(int activityId)
        {   
            int? loggedId = HttpContext.Session.GetInt32("loggedId");
            if(loggedId == null){
                return RedirectToAction("Index", "LoginReg");
            }
            
            Participant logged = _context.participants.SingleOrDefault(we => we.usersId == loggedId && we.activitiesId == activityId);
            _context.participants.Remove(logged);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public DateTime AddDuration(DateTime startDate, string duration)
        {
            string[] time = duration.Split(null);
            int num = Convert.ToInt32(time[0]);

            if(time[1] == "Days"){
                DateTime check = startDate.AddDays(num);
                return check;
            }
            if(time[1] == "Hours"){
                DateTime check = startDate.AddHours(num);
                return check;
            }
            if(time[1] == "Minutes"){
                DateTime check = startDate.AddMinutes(num);
                return check;
            }
            return startDate;
        }
    }
}
