using System;
using System.Configuration;

namespace CSI.Common.Config
{
    // Ref - https://refactoring.guru/design-patterns/singleton/csharp/example#example-1

    // The Singleton class defines the `GetInstance` method that serves as an
    // alternative to constructor and lets clients access the same instance of
    // this class over and over.

    // EN : The Singleton should always be a 'sealed' class to prevent class
    // inheritance through external classes and also through nested classes.
    public sealed class ChromeDriverConfig
    {
        public bool HideCommandPromptWindow { get; set; }
        public int ImplicitWaitSeconds { get; set; }

        // We now have a lock object that will be used to synchronize threads
        // during first access to the Singleton.
        private static readonly object Lock = new();

        // The Singleton's constructor should always be private to prevent
        // direct construction calls with the `new` operator.
        private ChromeDriverConfig()
        {
        }

        // The Singleton's instance is stored in a static field. 
        private static ChromeDriverConfig _instance;

        // This is the static method that controls the access to the singleton
        // instance. On the first run, it creates a singleton object and places
        // it into the static field. On subsequent runs, it returns the client
        // existing object stored in the static field.
        public static ChromeDriverConfig GetInstance()
        {
            // This conditional is needed to prevent threads stumbling over the
            // lock once the instance is ready.
            if (_instance == null)
            {
                // Now, imagine that the program has just been launched. Since
                // there's no Singleton instance yet, multiple threads can
                // simultaneously pass the previous conditional and reach this
                // point almost at the same time. The first of them will acquire
                // lock and will proceed further, while the rest will wait here.
                lock (Lock)
                {
                    // The first thread to acquire the lock, reaches this
                    // conditional, goes inside and creates the Singleton
                    // instance. Once it leaves the lock block, a thread that
                    // might have been waiting for the lock release may then
                    // enter this section. But since the Singleton field is
                    // already initialized, the thread won't create a new
                    // object.
                    if (_instance == null)
                    {
                        _instance = new ChromeDriverConfig
                        {
                            HideCommandPromptWindow = Convert.ToBoolean(ConfigurationManager.AppSettings["ChromeDriver:HideCommandPromptWindow"]),
                            ImplicitWaitSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["ChromeDriver:ImplicitWaitSeconds"])
                        };
                    }
                }
            }

            return _instance;
        }
    }
}
