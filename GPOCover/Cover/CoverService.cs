using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPOCover.Cover.Actions;
using GPOCover.Cover.Triggers;

namespace GPOCover.Cover;

public class CoverService
{
    readonly record struct Joke(string Setup, string Punchline);

    protected ILoggerFactory _loggerFactory;
    protected ILogger<CoverService> _logger;
    protected List<CoverConfiguration>? _config = null;
    internal List<TriggerRegistryChange>? _registryTriggers = null;
    internal List<TriggerDirectoryChange>? _directoryTriggers = null;
    internal List<LockFile>? _fileLocks = null;

    public CoverService(ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
        this._logger = this._loggerFactory.CreateLogger<CoverService>();
    }

    public void Configure(List<CoverConfiguration> configIn)
    {
        this._config = configIn;

        if (this._registryTriggers is null)
            this._registryTriggers = new List<TriggerRegistryChange>();
        if (this._directoryTriggers is null)
            this._directoryTriggers = new List<TriggerDirectoryChange>();
        if (this._fileLocks is null)
            this._fileLocks = new List<LockFile>();

        uint configurationId = 1;
        foreach (var config in this._config) {
            ++configurationId;
            switch (config.Trigger) {
                case Trigger.RegistryChange:
                    var triggerReg = new TriggerRegistryChange(configurationId, config.Path, this._loggerFactory);
                    this.AddRegistryChangeWatch(triggerReg, config);
                    break;

                case Trigger.FilesystemChange:
                    var dirInfo = new DirectoryInfo(config.Path);
                    var triggerDir = new TriggerDirectoryChange(configurationId, dirInfo, this._loggerFactory);
                    this.AddDirectoryChangeWatch(triggerDir, config);
                    break;

                case Trigger.FilesystemLock:
                    var fileInfo = new FileInfo(config.Path);
                    var lockFile = new LockFile(configurationId, fileInfo, this._loggerFactory);
                    this.AddFileLockWatch(lockFile, config);
                    break;

                default:
                    throw new NotImplementedException($"Cannot do trigger type {config.Trigger} yet!");
            }
        }
    }

    internal void AddRegistryChangeWatch(TriggerRegistryChange trigger, CoverConfiguration config)
    {
        if (this._registryTriggers is null)
            throw new ArgumentNullException(nameof(this._registryTriggers));
        if (this._registryTriggers.Contains(trigger))
            return;

        this._registryTriggers.Add(trigger);
        trigger.AddActions(config.Actions.Select(a => this._convertAction(a, config)).ToList<ActionBase>());
    }

    internal void AddDirectoryChangeWatch(TriggerDirectoryChange trigger, CoverConfiguration config)
    {
        if (this._directoryTriggers is null)
            throw new ArgumentNullException(nameof(this._directoryTriggers));
        if (this._directoryTriggers.Contains(trigger))
            return;

        this._directoryTriggers.Add(trigger);
        trigger.AddActions(config.Actions.Select(a => this._convertAction(a, config)).ToList<ActionBase>());
    }

    internal void AddFileLockWatch(LockFile trigger, CoverConfiguration config)
    {
        if (this._fileLocks is null)
            throw new ArgumentNullException(nameof(this._registryTriggers));
        if (this._fileLocks.Contains(trigger))
            return;

        this._fileLocks.Add(trigger);
        trigger.AddActions(config.Actions.Select(a => this._convertAction(a, config)).ToList<ActionBase>());
    }

    internal ActionBase _convertAction(CoverConfigurationAction action, CoverConfiguration config)
    {
        this._logger.LogDebug($"Converting action for: {config.Name}");

        if (action.Noop is not null)
            return new Noop(this._loggerFactory);
        if (action.Execute is not null)
        {
            if (String.IsNullOrEmpty(action.Execute.Command))
                throw new ArgumentException($"Execute-action for '{config.Name}' needs to have a command!");

            return new Execute(action.Execute.Command, action.Execute.Arguments, this._loggerFactory);
        }
        if (action.Sleep is not null)
            return new Sleep(action.Sleep.Value, this._loggerFactory);

        throw new NotImplementedException($"Unknown action type in YAML!");
    }

    public string GetJoke()
    {
        var joke = _jokes.ElementAt(Random.Shared.Next(_jokes.Count));

        return $"{joke.Setup}{Environment.NewLine}{joke.Punchline}";
    }

    // Programming jokes borrowed from:
    // https://github.com/eklavyadev/karljoke/blob/main/source/jokes.json
    private readonly HashSet<Joke> _jokes = new()
    {
        new Joke("What's the best thing about a Boolean?", "Even if you're wrong, you're only off by a bit."),
        new Joke("What's the object-oriented way to become wealthy?", "Inheritance"),
        new Joke("Why did the programmer quit their job?", "Because they didn't get arrays."),
        new Joke("Why do programmers always mix up Halloween and Christmas?", "Because Oct 31 == Dec 25"),
        new Joke("How many programmers does it take to change a lightbulb?", "None that's a hardware problem"),
        new Joke("If you put a million monkeys at a million keyboards, one of them will eventually write a Java program", "the rest of them will write Perl"),
        new Joke("['hip', 'hip']", "(hip hip array)"),
        new Joke("To understand what recursion is...", "You must first understand what recursion is"),
        new Joke("There are 10 types of people in this world...", "Those who understand binary and those who don't"),
        new Joke("Which song would an exception sing?", "Can't catch me - Avicii"),
        new Joke("Why do Java programmers wear glasses?", "Because they don't C#"),
        new Joke("How do you check if a webpage is HTML5?", "Try it out on Internet Explorer"),
        new Joke("A user interface is like a joke.", "If you have to explain it then it is not that good."),
        new Joke("I was gonna tell you a joke about UDP...", "...but you might not get it."),
        new Joke("The punchline often arrives before the set-up.", "Do you know the problem with UDP jokes?"),
        new Joke("Why do C# and Java developers keep breaking their keyboards?", "Because they use a strongly typed language."),
        new Joke("Knock-knock.", "A race condition. Who is there?"),
        new Joke("What's the best part about TCP jokes?", "I get to keep telling them until you get them."),
        new Joke("A programmer puts two glasses on their bedside table before going to sleep.", "A full one, in case they gets thirsty, and an empty one, in case they don’t."),
        new Joke("There are 10 kinds of people in this world.", "Those who understand binary, those who don't, and those who weren't expecting a base 3 joke."),
        new Joke("What did the router say to the doctor?", "It hurts when IP."),
        new Joke("An IPv6 packet is walking out of the house.", "He goes nowhere."),
        new Joke("3 SQL statements walk into a NoSQL bar. Soon, they walk out", "They couldn't find a table.")
    };

} // end class CoverService
