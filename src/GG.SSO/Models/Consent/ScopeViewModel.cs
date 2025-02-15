﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GG.SSO.Models.Consent
{
    public class ScopeViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool Checked { get; set; }
        public IEnumerable<ResourceViewModel> Resources { get; set; }
    }
}
