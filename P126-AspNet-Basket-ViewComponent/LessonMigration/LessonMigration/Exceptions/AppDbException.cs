using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LessonMigration.Exceptions
{
    public class AppDbException :Exception
    {
        public AppDbException(string message) :base(message) { }


    }
}
