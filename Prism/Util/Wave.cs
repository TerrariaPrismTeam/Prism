using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Prism.Util
{
    /*static class Adpcm
    {
        public const int Bitdepth = 4,
                         NCoeffs  = 7;
    }*/

    enum WaveMagic : uint
    {
        // assuming little-endian
        // (lots of code in vanilla does as well, so we don't need to
        // make this portable)
        RIFF = 0x46464952,
        WAVE = 0x45564157,
        fmt_ = 0x20746D66,
        data = 0x61746164,
        //fact = 0x74636166
    }

    enum WaveFormat : ushort
    {
        Unknown   = 0x0000,
        PCM   = 0x0001,
        MicrosoftADPCM = 0x0002, // ADPCM (MS)
        PCMFloat = 0x0003,
        IbmCVSD  = 0x0005, // IBM CVSD
        ALaw  = 0x0006, // A-law
        MuLaw = 0x0007, // µ-law
        OkiADPCM = 0x0010, // ADPCM (OKI)
        IntelADPCM = 0x0011, // ADPCM (Intel)
        MediaspaceADPCM = 0x0012, // ADPCM (Mediaspace)
        SierraADPCM = 0x0013, // ADPCM (Sierra)
        ADPCM_G723 = 0x0014, // ADPCM (G.723)
        DistD = 0x0015, // ?
        ItuADPCM_G723 = 0x0016, // ADPCM (ITU, G.723)
        DialogicOkiADPCM = 0x0017, // ADPCM (Dialogic OKI)
        YamahaADPCM = 0x0020, // ADPCM (Yamaha)
        Sonarc = 0x0021, // Sonarc
        TrueSpeech = 0x0022, // TrueSpeech (DSP Group)
        EchoSC1 = 0x0023, // Echo SC 1
        AF36  = 0x0024, // AF36 (AudioFile)
        APTX  = 0x0025, // APTX
        AF10  = 0x0026, // AF10 (AudioFile)
        DolbyAAC = 0x0030, // AAC (Dolby)
        MicrosoftGSM = 0x0031, // GSM (MS)
        AntexADPCME = 0x0033, // ADPCME (Antex)
        ControlVQLPC = 0x0034, // VQLPC (Control Resources)
        DigiReal  = 0x0035, // DigiReal
        RockwellADPCM = 0x0036, // ADPCM (Rockwell)
        ControlCR10  = 0x0037, // CR10 (Control Resources)
        NaturalVBXADPCM = 0x0038, // VBXADPCM (Natural MicroSystems)
        RockwellADPCM2 = 0x003B, // ADPCM (Rockwell, again?)
        RockwellDigiTalk = 0x003C, // DigiTalk (Rockwell)
        ItuADPCM_G721 = 0x0040, // ADPCM (ITU, G.721)
        CELP_G728 = 0x0041, // G.728 (CELP)
        Microsoft_G723 = 0x0042, // G.723 (MS)
        ItuT_G726 = 0x0045, // G.726 (ITU-T)
        MPEG  = 0x0050, // MPEG
        MPEG3 = 0x0055, // MPEG-3
        ApicomADPCM_G726 = 0x0064, // ADPCM (APICOM, G.726)
        ApicomADPCM_G722 = 0x0065, // ADPCM (APICOM, G.722)
        Xma2  = 0x0069, // XMA 2 (Microsoft)
        IbmA = 0x0101, // A-law (IBM)
        IbmMu  = 0x0102, // µ-law (IBM)
        IbmADPCM = 0x0103, // ADPCM (IBM)
        CreativeADPCM = 0x0200, // ADPCM (Creative)
        TOWNS = 0x0300, // FM TOWNS SND (Fujitsu)
        OliGSM = 0x1000, // GSM (Olivetti)
        OliADPCM = 0x1001, // ADPCM (Olivetti)
        OliCELP = 0x1002, // CELP (Olivetti)
        OliSBC = 0x1003, // SBC (Olivetti)
        OliOPR = 0x1004, // OPR (Olivetti)
        EXT   = 0xFFFE
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Riff
    {
        public WaveMagic magic;
        public uint size;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct WaveHeader
    {
        public Riff riff;
        public WaveMagic wavemagic;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct WaveFmt
    {
        public WaveFormat format;
        public ushort channels, samplerate, byterate, blockalign, bitdepth;
    }

    /*[StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct WaveADPCMCB
    {
        public ushort samples_block, ncoeffs;
        public fixed short coeffs[Adpcm.NCoeffs << 1];
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct WaveFmtADPCM
    {
        public uint cbsize;
        public WaveADPCMCB cb;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct WaveFactADPCM
    {
        public Riff header;
        public uint data;
    }*/

    unsafe static class Wave
    {
        /*static short[] AdpcmCoeffs =
        {
            0x0100,  0x0000,
            0x0200, -0x0100,
            0x0000,  0x0000,
            0x00C0,  0x0040,
            0x00F0,  0x0000,
            0x01CC, -0x00D0,
            0x0188, -0x00E8
        };*/
    }
}

