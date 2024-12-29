using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AVFoundation;
using Whisper.net;
using Whisper.net.Ggml;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Media;
using System.Globalization;

// #if IOS
// using AVFoundation;
// #endif

namespace KSpeackEnglish.Components;

class HomePageState
{
  public int Counter { get; set; }
  public string Transcription { get; set; } = String.Empty;
}

partial class HomePage : Component<HomePageState>
{
  [Inject]
  ISpeechToText _speechToText;

  public override VisualNode Render()
    => ContentPage(
        ScrollView(
            VStack(
              Image("dotnet_bot.png")
                .HeightRequest(200)
                .HCenter()
                .Set(SemanticProperties.DescriptionProperty, "Cute dot net bot waving hi to you!"),

              Button("Start Listening")
                .OnClicked(HandleOnClickedStartListening)
                .HCenter(),

              Button("Stop Listening")
                .OnClicked(HandleOnClickedStopListening)
                .HCenter(),

              Button("Transcript")
                .OnClicked(HandleOnClickedTranscript)
                .HCenter(),

              Label(State.Transcription)
                .FontSize(18)
                .HCenter(),

              Button("Texto to Speech")
                .OnClicked(HandleOnClickedTextToSpeech)
                .HCenter()
            )
        )
        .VCenter()
      );

  private async void HandleOnClickedStartListening()
  {
    try
    {
      var isGranted = await _speechToText.RequestPermissions(CancellationToken.None);
      if (!isGranted)
      {
        await Toast.Make("Permission not granted").Show(CancellationToken.None);
        return;
      }

      _speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
      _speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
      await _speechToText.StartListenAsync(CultureInfo.CurrentCulture, CancellationToken.None);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      await Toast.Make(ex.Message).Show(CancellationToken.None);
    }
  }

  void OnRecognitionTextUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)
  {
    SetState(s => s.Transcription = s.Transcription + args.RecognitionResult + " ");
  }

  void OnRecognitionTextCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
  {
    SetState(s => s.Transcription = args.RecognitionResult);
  }

  private async void HandleOnClickedStopListening()
  {
    await _speechToText.StopListenAsync(CancellationToken.None);
    _speechToText.RecognitionResultUpdated -= OnRecognitionTextUpdated;
    _speechToText.RecognitionResultCompleted -= OnRecognitionTextCompleted;
  }

  private async void HandleOnClickedTranscript()
  {
    // Caminho do arquivo de áudio (substitua pelo caminho real)
    string caminhoArquivo = "kennedy.wav";

    try
    {
      // Verifica se o arquivo existe
      if (!File.Exists(caminhoArquivo))
      {
        throw new FileNotFoundException("Arquivo de áudio não encontrado.", caminhoArquivo);
      }

      using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(GgmlType.Tiny, QuantizationType.Q4_0);
      using var memoryStream = new MemoryStream();
      await modelStream.CopyToAsync(memoryStream);

      using var mauiStream = await FileSystem.OpenAppPackageFileAsync(caminhoArquivo);
      var audioFileStream = new MemoryStream();

      await mauiStream.CopyToAsync(audioFileStream);
      audioFileStream.Seek(0, SeekOrigin.Begin);

      using var whisperFactory = WhisperFactory.FromBuffer(memoryStream.ToArray());
      var whisperBuilder = whisperFactory.CreateBuilder();
      using var whisperProcessor = whisperBuilder.Build();

      var transcription = string.Empty;
      SetState(s => s.Transcription = transcription);
      await foreach (var result in whisperProcessor.ProcessAsync(audioFileStream))
      {
        transcription = $"{transcription}{Environment.NewLine}{result.Text}";
      }
      SetState(s => s.Transcription = transcription);
    }
    catch (Exception ex)
    {
      SetState(s => s.Transcription = ex.Message);
    }
  }

  private async void HandleOnClickedTextToSpeech()
  {
    try
    {
      await TextToSpeech.SpeakAsync(State.Transcription);
      // #if IOS
      // var speechSynthesizer = new AVSpeechSynthesizer();
      // var speechUtterance = new AVSpeechUtterance(State.Transcription)
      // {
      //     Rate = AVSpeechUtterance.MaximumSpeechRate / 2,
      //     Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
      //     Volume = 0.75f,
      //     PitchMultiplier = 1.0f
      // };

      // speechSynthesizer.SpeakUtterance(speechUtterance);
      // #endif
    }
    catch (Exception ex)
    {
      SetState(s => s.Transcription = ex.Message);
    }
  }
}
