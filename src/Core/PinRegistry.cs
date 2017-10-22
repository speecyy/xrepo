using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XRepo.Core
{
    public class PinRegistry : JsonRegistry<PinHolder>
    {
        public static PinRegistry ForDirectory(string directoryPath)
        {
            return Load<PinRegistry>(directoryPath);
        }

        protected override string Filename
        {
            get { return "pin.registry"; }
        }

        public AssemblyPin PinAssembly(string assemblyName)
        {
            if(Data.Assemblies.Contains(assemblyName))
                throw new InvalidOperationException($"The assembly '{assemblyName}' is already pinned.");

            var pin = new AssemblyPin(assemblyName);
            Data.Assemblies.Add(pin);

            return pin;
        }

        public bool IsAssemblyPinned(string assemblyName)
        {
            return Data.Assemblies.Contains(assemblyName);
        }

        public AssemblyPin UnpinAssembly(string assemblyName)
        {
            if (Data.Assemblies.Contains(assemblyName))
            {
                var pinToRemove = Data.Assemblies[assemblyName];
                Data.Assemblies.Remove(pinToRemove);
                return pinToRemove;
            }

            return null;
        }

        public IEnumerable<AssemblyPin> GetPinnedAssemblies()
        {
            return Data.Assemblies;
        }

        public bool IsPackagePinned(string packageId)
        {
            return Data.Packages.Contains(packageId);
        }

        public PackagePin GetPackagePin(string packageId)
        {
            return Data.Packages[packageId];
        }

        public PackagePin PinPackage(string packageId)
        {
            if(Data.Packages.Contains(packageId))
                throw new InvalidOperationException($"The package '{packageId}' is already pinned");

            var pin = new PackagePin(packageId);
            Data.Packages.Add(pin);

            return pin;
        }

        public PackagePin UnpinPackage(string packageId)
        {
            if(Data.Packages.Contains(packageId))
            {
                var pinToRemove = Data.Packages[packageId];
                Data.Packages.Remove(pinToRemove);
                return pinToRemove;
            }

            return null;
        }

        public IEnumerable<PackagePin> GetPinnedPackages()
        {
            return Data.Packages;
        }

        public RepoPin PinRepo(string repoName)
        {
            if (Data.Repos.Contains(repoName))
                throw new InvalidOperationException($"The repo '{repoName}' is already pinned");

            var pin = new RepoPin(repoName);
            Data.Repos.Add(pin);

            return pin;
        }

        public bool IsRepoPinned(string repoName)
        {
            return Data.Repos.Contains(repoName);
        }

        public RepoPin GetRepoPin(string repoName)
        {
            return Data.Repos[repoName];
        }

        public Pin GetAssemblyPin(string assemblyName)
        {
            return Data.Assemblies[assemblyName];
        }

        public RepoPin UnpinRepo(string repoName)
        {
            if (Data.Repos.Contains(repoName))
            {
                var pinToRemove = Data.Repos[repoName];
                Data.Repos.Remove(repoName);

                return pinToRemove;
            }

            return null;
        }

        public IEnumerable<Pin> UnpinAll()
        {
            var pins = new List<Pin>();
            pins.AddRange(Data.Repos);
            pins.AddRange(Data.Packages);
            pins.AddRange(Data.Assemblies);
            Data.Repos.Clear();
            Data.Assemblies.Clear();
            Data.Packages.Clear();

            return pins;
        }

        public IEnumerable<RepoPin> GetPinnedRepos()
        {
            return Data.Repos;
        }
    }

    public class PinHolder
    {
        public AssemblyPinCollection Assemblies { get; private set; }
        public PackagePinCollection Packages { get; private set; }
        public RepoPinCollection Repos { get; private set; }

        public PinHolder()
        {
            Assemblies = new AssemblyPinCollection();
            Packages = new PackagePinCollection();
            Repos = new RepoPinCollection();
        }
    }

    public class AssemblyPin : Pin
    {
        public AssemblyPin(string name) : base(name)
        {
            
        }
        
        public override string Description => $"The assembly '{Name}' has been pinned. All references to this assembly will now be resolved to local copies.";
    }

    public class AssemblyPinCollection : KeyedCollection<string,AssemblyPin>
    {
        public AssemblyPinCollection() : base(StringComparer.OrdinalIgnoreCase) {}

        protected override string GetKeyForItem(AssemblyPin item)
        {
            return item.Name;
        }
    }

    public class PackagePin : Pin
    {
        public PackagePin(string packageId) : base(packageId)
        {
            
        }

        public string PackageId => Name;

        public override string Description => $"The package '{Name}' has been pinned. All references to this package will now be resolved to local copies.";
    }

    public class PackagePinCollection : KeyedCollection<string,PackagePin>
    {
        protected override string GetKeyForItem(PackagePin item)
        {
            return item.PackageId;
        }
    }

    public class RepoPin : Pin
    {
        public RepoPin(string name) : base(name)
        {
            
        }

        public override string Description => $"The repo {Name} has been pinned. All references to packages and assemblies built within this repo will now be resolved to local copies.";
    }

    public class RepoPinCollection : KeyedCollection<string,RepoPin>
    {
        public RepoPinCollection() : base(StringComparer.OrdinalIgnoreCase) {}
        
        protected override string GetKeyForItem(RepoPin item)
        {
            return item.Name;
        }
    }
}