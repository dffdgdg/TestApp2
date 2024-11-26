    using System;
    using System.Collections.Generic;

    namespace TestApp.Model;

    public partial class Question
    {
        public int Id { get; set; }

        public int Test { get; set; }

        public string Name { get; set; } = null!;

        public byte[]? Image { get; set; }

        public int Type { get; set; }

        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

        public virtual Test TestNavigation { get; set; } = null!;
    }
