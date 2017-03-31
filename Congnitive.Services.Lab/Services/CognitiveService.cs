using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Plugin.Media.Abstractions;

namespace Cognitive.Services.Lab
{
	public class CognitiveService
	{
		public static CognitiveService Instance { get; } = new CognitiveService();

		public IFaceServiceClient FaceServiceClient { get; set; }
		public EmotionServiceClient EmotionServiceClient { get; set; }
		public string PersonGroupId { get; set; }

		public CognitiveService()
		{
			// Face APIs
			FaceServiceClient = new FaceServiceClient("25cb663824684ae88357c0f7f0c460ac");
			// Emotion APIs
			EmotionServiceClient = new EmotionServiceClient("f8b92cbe5d704194a05df3f1322a00e6");
		}

		/// <summary>
		/// Detecta los rostros y las emociones de una imagen
		/// </summary>
		/// <returns>The face and emotions async.</returns>
		/// <param name="inputFile">Input file.</param>
		public async Task<FaceEmotionDetection> DetectFaceAndEmotionsAsync(string inputFile)
		{
			// Obtenemos las emociones para la imagen
			var emotionResult = await EmotionServiceClient.RecognizeAsync(inputFile);

			// Se asume que en la imagen hay solo un rostro, se retorna las emociones para el primer resultado
			var faceEmotion = emotionResult[0]?.Scores.ToRankedList();

			// Creamos la lista con los atributos que queremos analizar
			var requiredFaceAttributes = new FaceAttributeType[] {
				FaceAttributeType.Age,
				FaceAttributeType.Gender,
				FaceAttributeType.Smile,
				FaceAttributeType.FacialHair,
				FaceAttributeType.HeadPose,
				FaceAttributeType.Glasses
				};

			// Obtenemos la lista de rostros en la imagen
			var faces = await FaceServiceClient.DetectAsync(inputFile, false, false, requiredFaceAttributes);

			// Asumimos que hay solo un rostro en la foto y obtenemos los atributos
			var faceAttributes = faces[0]?.FaceAttributes;

			if (faceEmotion == null || faceAttributes == null) return null;

			return new FaceEmotionDetection
			{
				Age = faceAttributes.Age,
				Gender = faceAttributes.Gender,
				Glasses = faceAttributes.Glasses.ToString(),
				Smile = faceAttributes.Smile,
				Beard = faceAttributes.FacialHair.Beard,
				Emotion = faceEmotion.FirstOrDefault().Key,
				Moustache = faceAttributes.FacialHair.Moustache
			};
		}


		/// <summary>
		/// Crea la persona y le asigna un rostro.
		/// </summary>
		/// <returns>The person face async.</returns>
		/// <param name="name">Name.</param>
		/// <param name="fileUrl">File URL.</param>
		public async Task AddPersonFaceAsync(string name, string fileUrl)
		{
			if (string.IsNullOrWhiteSpace(PersonGroupId))
			{
				PersonGroupId = Guid.NewGuid().ToString();
				await FaceServiceClient.CreatePersonGroupAsync(PersonGroupId, "Friends");
			}

			var p = await FaceServiceClient.CreatePersonAsync(PersonGroupId, name);
			await FaceServiceClient.AddPersonFaceAsync(PersonGroupId, p.PersonId, fileUrl);

			await TrainPersonGroup();
		}

		/// <summary>
		/// Entrena el algoritmo de identificacion con los rostros del grupo.
		/// </summary>
		/// <returns>The person group.</returns>
		public async Task TrainPersonGroup()
		{
			try
			{
				await FaceServiceClient.TrainPersonGroupAsync(PersonGroupId);
				TrainingStatus trainingStatus = null;

				while (true)
				{
					trainingStatus = await FaceServiceClient.GetPersonGroupTrainingStatusAsync(PersonGroupId);

					if (trainingStatus.Status != Status.Running)
						break;

					await Task.Delay(1000);
				}
			}
			catch
			{
				return;
			}
		}

		/// <summary>
		/// Identifica los rostros de las personas del grupo en la foto.
		/// </summary>
		/// <returns>The face async.</returns>
		/// <param name="file">File.</param>
		public async Task<List<string>> IdentifyFaceAsync(string file)
		{
			var result = new List<string>();
			var faces = await FaceServiceClient.DetectAsync(file);
			var faceIds = faces.Select(face => face.FaceId).ToArray();

			var results = await FaceServiceClient.IdentifyAsync(PersonGroupId, faceIds);
			foreach (var identifyResult in results)
			{
				if (identifyResult.Candidates.Length == 0)
				{
					result.Add("Unknown");
				}
				else
				{
					// Obtenemos el primer candidato de la lista retornada
					var candidateId = identifyResult.Candidates[0].PersonId;
					var person = await FaceServiceClient.GetPersonAsync(PersonGroupId, candidateId);
					result.Add(person.Name);
				}
			}
			return result;
		}
	}
}
