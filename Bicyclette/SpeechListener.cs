using System;
using System.Speech.Recognition;

namespace Bicyclette
{
    public class SpeechListener
    {
        private SpeechRecognitionEngine recognizer;

        public event Action<string> OnSpeechRecognized;

        public bool IsListening => recognizer != null;

        public void Start()
        {
            if (recognizer != null) return;

            recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();

            recognizer.LoadGrammar(new DictationGrammar());

            recognizer.SpeechRecognized += (s, e) =>
            {
                string text = e.Result.Text;
                OnSpeechRecognized?.Invoke(text);
            };

            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void Stop()
        {
            if (recognizer == null) return;

            recognizer.RecognizeAsyncStop();
            recognizer.Dispose();
            recognizer = null;
        }
    }
}

