using System;
using System.Collections.Generic;
using System.Text;

namespace Models.LiveAnimalModels
{
    public class IndexViewModel
    {
        public List<LiveAnimalViewModelFrontend> Featured { get; set; }
        public List<LiveAnimalViewModelFrontend> Latest { get; set; }
    }
}
