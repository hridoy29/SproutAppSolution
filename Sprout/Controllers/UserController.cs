using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DbExecutor;
using SproutEntity;
using SproutDAL;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Description;

namespace Sprout.Controllers
    
{
    public class UserController : ApiController
    {
        // GET: api/User
        public IList<User> Get()
        {
            List<User> usersList = new List<User>();
            try
            {
                usersList = Facade.UsersDAO.Get();
                return usersList;
            }
            catch (Exception ex)
            {
                return usersList;
            }
             
        }

        // GET: api/User/5
        public Users Get(int id)
        {
            Users user = new Users();
            try
            {


             
                user = Facade.UsersDAO.Get(id);
               
                return user;
            }
            catch (Exception ex)
            {
                return user;
            }
           
        }

        // POST: api/User

        [ResponseType(typeof(User))]
        public IHttpActionResult Post(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           string ret= Facade.UsersDAO.Post(user, "INSERT");
            if(ret== "Users already exist")
            {
                user.MobileNo = ret;
                user.Id = 0;
                return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
            }
           

        }


        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }



       
    }
}
