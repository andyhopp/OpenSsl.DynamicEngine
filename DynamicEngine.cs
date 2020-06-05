using System.Runtime.InteropServices;

namespace System.Security.Cryptography.OpenSsl
{
    public class DynamicEngine : IDisposable
    {
        private DynamicEngineHandle engine;
        public DynamicEngine(string name)
        {
            Name = name;
        }
        public string Id
        {
            get
            {
                if (engine.IsClosed || engine.IsInvalid) throw new InvalidOperationException();
                var id = SafeNativeMethods.ENGINE_get_id(engine);
                return Marshal.PtrToStringAuto(id);
            }
        }
        public string Name { get; private set; }

        public void Initialize()
        {
            if (null == engine)
            {
                engine = SafeNativeMethods.ENGINE_by_id(Name);
                if (engine.IsInvalid)
                {
                    throw new InvalidOperationException($"Unable to load engine '{Name}'");
                }
                var result = SafeNativeMethods.ENGINE_init(engine);
                if (0 == result)
                {
                    SafeNativeMethods.ENGINE_free(engine);
                    throw new InvalidOperationException($"Unable to load engine '{Name}'. ENGINE_init returned {result}");
                }
            }
        }

        public void Finish()
        {
            SafeNativeMethods.ENGINE_finish(engine);
        }

        public void SetDefaults(EngineDefaults defaults)
        {
            if (defaults == EngineDefaults.All ||
                defaults == (defaults & (
                    EngineDefaults.RSA |
                    EngineDefaults.DSA |
                    EngineDefaults.DH |
                    EngineDefaults.RandomNumberGeneration |
                    EngineDefaults.ECDH |
                    EngineDefaults.ECDSA |
                    EngineDefaults.Ciphers |
                    EngineDefaults.Digests |
                    EngineDefaults.Store |
                    EngineDefaults.PKEY_METHS |
                    EngineDefaults.PKEY_ASN1_METHS)))
            {
                var result = SafeNativeMethods.ENGINE_set_default(engine, defaults);
                if (0 == result)
                {
                    SafeNativeMethods.ENGINE_free(engine);
                    throw new InvalidOperationException($"Unable to set engine as default '{defaults}'. ENGINE_set_default returned {result}");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(defaults));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                engine.Dispose();
            }
        }
    }
}
