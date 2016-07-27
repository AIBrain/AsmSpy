namespace AsmSpy {
    using System;
    using System.Reflection;

    public class ReferencedAssembly
    {
        public Version VersionReferenced { get; }
        public Assembly ReferencedBy { get; }

        public ReferencedAssembly(Version versionReferenced, Assembly referencedBy)
        {
            this.VersionReferenced = versionReferenced;
            this.ReferencedBy = referencedBy;
        }
    }
}