using GymSystem.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Entities
{
    public class Trainer:GymUser
    {
        public Specailties Specailtize { get; set; }
        public DateTime HiringDate { get; set; }
        public ICollection<Session> Sessions = new HashSet<Session>();
    }
}
