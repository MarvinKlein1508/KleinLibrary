namespace KleinLibrary
{
    public class Pipeline
    {
        /// <summary>
        /// Der aktuelle Index des zu durchlaufenden Pipeline Teils
        /// </summary>
        private int Current { get; set; } = 0;

        private bool KeepRunning { get; set; } = true;

        private bool Running { get; set; } = false;
        private bool Cancelled { get; set; } = false;

        /// <summary>
        /// Speichert alle Synchronen Pipeline Funktionen
        /// </summary>
        private Dictionary<int, Func<bool>> SynchronousPipes { get; set; } = new Dictionary<int, Func<bool>>();

        /// <summary>
        /// Speichert alle Asynchronen Pipeline Funktionen
        /// </summary>
        private Dictionary<int, Func<Task<bool>>> AsynchronousPipes { get; set; } = new Dictionary<int, Func<Task<bool>>>();

        /// <summary>
        /// Fügt der Pipeline eine asynchrone Funktion hinzu
        /// </summary>
        /// <param name="tmpFunc">Die Asynchrone Funktion die der Pipeline hinzugefügt werden soll</param>
        public void Add(Func<Task<bool>> tmpFunc)
        {
            AsynchronousPipes.Add(NextId(), tmpFunc);
        }

        /// <summary>
        /// Fügt der Pipeline eine synchrone Funktion hinzu
        /// </summary>
        /// <param name="tmpFunc">Die synchrone Funktion die der Pipeline hinzugefügt werden soll</param>
        public void Add(Func<bool> tmpFunc)
        {
            SynchronousPipes.Add(NextId(), tmpFunc);
        }

        /// <summary>
        /// Gibt die nächste einzufügende ID zurück
        /// </summary>
        /// <returns>Gibt einen <see cref="int"/> zurück der den Index des nächsten Pipeline Part beinhaltet</returns>
        private int NextId()
        {
            int maxSync = -1;
            int maxAsync = -1;
            if (SynchronousPipes.Count > 0)
            {
                maxSync = SynchronousPipes.Max(x => x.Key);
            }

            if (AsynchronousPipes.Count > 0)
            {
                maxAsync = AsynchronousPipes.Max(x => x.Key);
            }

            if (maxSync == -1 && maxAsync == -1)
            {
                return 0;
            }

            return maxSync > maxAsync ? maxSync + 1 : maxAsync + 1;
        }

        /// <summary>
        /// Führt den nächsten Teil der Pipeline asynchron aus
        /// </summary>
        /// <param name="value">Der Wert der an die Piepline Teile übergeben werden soll</param>
        /// <returns>Gibt einen <see cref="Task"/> of <see cref="bool"/> zurück der angibt ob dieser Teil erfolg hatte</returns>
        private Task<bool> NextAsync()
        {
            Task<bool> result = Task.FromResult(true);
            if (SynchronousPipes.ContainsKey(Current))
            {
                var kvp = SynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    result = Task.FromResult(kvp.Value.Invoke());
                }
            }
            else if (AsynchronousPipes.ContainsKey(Current))
            {
                var kvp = AsynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    result = kvp.Value.Invoke();
                }
            }
            Current++;
            return result;
        }

        /// <summary>
        /// Führt den nächsten Teil der Pipeline synchron aus
        /// </summary>
        /// <param name="value">Der Wert der an die Pipeline Teile übergeben werden soll</param>
        /// <returns></returns>
        private bool Next()
        {
            bool result = true;
            if (SynchronousPipes.ContainsKey(Current))
            {
                var kvp = SynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    result = kvp.Value.Invoke();
                }
            }
            else if (AsynchronousPipes.ContainsKey(Current))
            {
                var kvp = AsynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    Task<bool> t = kvp.Value.Invoke();
                    t.RunSynchronously();
                    result = t.Result;
                }
            }
            Current++;
            return result;
        }

        /// <summary>
        /// Lässt die Pipeline solange laufen, bis ein Pipeline Teil einen Misserfolg meldet
        /// </summary>
        /// <param name="value">Der Wert der an die Pipeline Teile übergeben werden soll</param>
        /// <returns>Gibt einen <see cref="bool"/> zurück der angibt ob die Pipeline bis zum ende laufen konnte</returns>
        public bool RunUntilFailure()
        {
            Running = true;
            for (int i = 0; i < (SynchronousPipes.Count + AsynchronousPipes.Count); i++)
            {
                if (!KeepRunning)
                {
                    Cancelled = true;
                    Running = false;
                    return false;
                }


                if (!Next())
                {
                    Running = false;
                    return false;
                }
            }
            Running = false;
            return true;
        }

        /// <summary>
        /// Lässt die Pipeline solange asynchron laufen, bis ein Pipeline Teil einen Misserfolg meldet
        /// </summary>
        /// <param name="value">Der Wert der an die Pipeline Teile übergeben werden soll</param>
        /// <returns>Gibt einen <see cref="Task"/> of <see cref="bool"/> zurück der angibt ob die Pipeline bis zum ende laufen konnte</returns>
        public async Task<bool> RunUntilFailureAsync()
        {
            Running = true;
            for (int i = 0; i < (SynchronousPipes.Count + AsynchronousPipes.Count); i++)
            {
                if (!KeepRunning)
                {
                    Cancelled = true;
                    Running = false;
                    return false;
                }


                if (!await NextAsync())
                {
                    Running = false;
                    return false;
                }
            }
            Running = false;
            return true;
        }

        /// <summary>
        /// Entfernt alle Funktionen aus der Pipeline
        /// </summary>
        public void Clear()
        {
            SynchronousPipes.Clear();
            AsynchronousPipes.Clear();
        }

        /// <summary>
        /// Setzt den Zähler der Pipeline zurück
        /// </summary>
        public void Reset()
        {
            Current = 0;
            KeepRunning = true;
        }

        public async Task HardReset()
        {
            await WaitForCancelAsync();
            Clear();
            Reset();
        }

        public async Task WaitForCancelAsync()
        {
            if (!Running)
            {
                return;
            }

            KeepRunning = false;

            while (!Cancelled && Running)
            {
                await Task.Delay(100);
            }
            return;
        }
    }

    /// <summary>
    /// Läuft alle enthaltenen Funktionen nacheinander durch. 
    /// </summary>
    /// <typeparam name="T">Der Typ der an die Funktionen übergeben werden soll</typeparam>
    public class Pipeline<T>
    {
        /// <summary>
        /// Der aktuelle Index des zu durchlaufenden Pipeline Teils
        /// </summary>
        public int Current { get; private set; } = 0;

        private bool KeepRunning { get; set; } = true;

        private bool Running { get; set; } = false;
        private bool Cancelled { get; set; } = false;

        /// <summary>
        /// Speichert alle Synchronen Pipeline Funktionen
        /// </summary>
        private Dictionary<int, Func<T, bool>> SynchronousPipes { get; set; } = new Dictionary<int, Func<T, bool>>();

        /// <summary>
        /// Speichert alle Asynchronen Pipeline Funktionen
        /// </summary>
        private Dictionary<int, Func<T, Task<bool>>> AsynchronousPipes { get; set; } = new Dictionary<int, Func<T, Task<bool>>>();

        /// <summary>
        /// Fügt der Pipeline eine asynchrone Funktion hinzu
        /// </summary>
        /// <param name="tmpFunc">Die Asynchrone Funktion die der Pipeline hinzugefügt werden soll</param>
        public void Add(Func<T, Task<bool>> tmpFunc)
        {
            AsynchronousPipes.Add(NextId(), tmpFunc);
        }

        /// <summary>
        /// Fügt der Pipeline eine synchrone Funktion hinzu
        /// </summary>
        /// <param name="tmpFunc">Die synchrone Funktion die der Pipeline hinzugefügt werden soll</param>
        public void Add(Func<T, bool> tmpFunc)
        {
            SynchronousPipes.Add(NextId(), tmpFunc);
        }

        /// <summary>
        /// Gibt die nächste einzufügende ID zurück
        /// </summary>
        /// <returns>Gibt einen <see cref="int"/> zurück der den Index des nächsten Pipeline Part beinhaltet</returns>
        private int NextId()
        {
            int maxSync = -1;
            int maxAsync = -1;
            if (SynchronousPipes.Count > 0)
            {
                maxSync = SynchronousPipes.Max(x => x.Key);
            }

            if (AsynchronousPipes.Count > 0)
            {
                maxAsync = AsynchronousPipes.Max(x => x.Key);
            }

            if (maxSync == -1 && maxAsync == -1)
            {
                return 0;
            }

            return maxSync > maxAsync ? maxSync + 1 : maxAsync + 1;
        }

        /// <summary>
        /// Führt den nächsten Teil der Pipeline asynchron aus
        /// </summary>
        /// <param name="value">Der Wert der an die Piepline Teile übergeben werden soll</param>
        /// <returns>Gibt einen <see cref="Task"/> of <see cref="bool"/> zurück der angibt ob dieser Teil erfolg hatte</returns>
        private Task<bool> NextAsync(T value)
        {
            Task<bool> result = Task.FromResult(true);
            if (SynchronousPipes.ContainsKey(Current))
            {
                var kvp = SynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    result = Task.FromResult(kvp.Value.Invoke(value));
                }
            }
            else if (AsynchronousPipes.ContainsKey(Current))
            {
                var kvp = AsynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    result = kvp.Value.Invoke(value);
                }
            }
            Current++;
            return result;
        }

        /// <summary>
        /// Führt den nächsten Teil der Pipeline synchron aus
        /// </summary>
        /// <param name="value">Der Wert der an die Pipeline Teile übergeben werden soll</param>
        /// <returns></returns>
        private bool Next(T value)
        {
            bool result = true;
            if (SynchronousPipes.ContainsKey(Current))
            {
                var kvp = SynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    result = kvp.Value.Invoke(value);
                }
            }
            else if (AsynchronousPipes.ContainsKey(Current))
            {
                var kvp = AsynchronousPipes.Where(x => x.Key == Current).First();
                if (kvp.Value != default && kvp.Value is not null)
                {
                    Task<bool> t = kvp.Value.Invoke(value);
                    t.RunSynchronously();
                    result = t.Result;
                }
            }
            Current++;
            return result;
        }

        /// <summary>
        /// Lässt die Pipeline solange laufen, bis ein Pipeline Teil einen Misserfolg meldet
        /// </summary>
        /// <param name="value">Der Wert der an die Pipeline Teile übergeben werden soll</param>
        /// <returns>Gibt einen <see cref="bool"/> zurück der angibt ob die Pipeline bis zum ende laufen konnte</returns>
        public bool RunUntilFailure(T value)
        {
            Running = true;
            for (int i = 0; i < (SynchronousPipes.Count + AsynchronousPipes.Count); i++)
            {
                if (!KeepRunning)
                {
                    Cancelled = true;
                    Running = false;
                    return false;
                }


                if (!Next(value))
                {
                    Running = false;
                    return false;
                }
            }
            Running = false;
            return true;
        }

        /// <summary>
        /// Lässt die Pipeline solange asynchron laufen, bis ein Pipeline Teil einen Misserfolg meldet
        /// </summary>
        /// <param name="value">Der Wert der an die Pipeline Teile übergeben werden soll</param>
        /// <returns>Gibt einen <see cref="Task"/> of <see cref="bool"/> zurück der angibt ob die Pipeline bis zum ende laufen konnte</returns>
        public async Task<bool> RunUntilFailureAsync(T value)
        {
            Running = true;
            for (int i = 0; i < (SynchronousPipes.Count + AsynchronousPipes.Count); i++)
            {
                if (!KeepRunning)
                {
                    Cancelled = true;
                    Running = false;
                    return false;
                }


                if (!await NextAsync(value))
                {
                    Running = false;
                    return false;
                }
            }
            Running = false;
            return true;
        }

        /// <summary>
        /// Entfernt alle Funktionen aus der Pipeline
        /// </summary>
        public void Clear()
        {
            SynchronousPipes.Clear();
            AsynchronousPipes.Clear();
        }

        /// <summary>
        /// Setzt den Zähler der Pipeline zurück
        /// </summary>
        public void Reset()
        {
            Current = 0;
            KeepRunning = true;
        }

        public async Task HardReset()
        {
            await WaitForCancelAsync();
            Clear();
            Reset();
        }

        public async Task WaitForCancelAsync()
        {
            if (!Running)
            {
                return;
            }

            KeepRunning = false;

            while (!Cancelled && Running)
            {
                await Task.Delay(100);
            }
            return;
        }
    }

    /// <summary>
    /// Stellt ein Interface zur Verfügung, um aufeinander abhängige Abläufe darzustellen.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPipeline<T>
    {
        /// <summary>
        /// Ruft die Pipeline mit den Abläufen ab, oder legt diese fest.
        /// </summary>
        Pipeline<T> Pipeline { get; set; }
        /// <summary>
        /// Legt alle Aufrufe der <see cref="Pipeline"/> fest.
        /// </summary>
        /// <returns></returns>
        Task SetupPipelineAsync();
    }
}
