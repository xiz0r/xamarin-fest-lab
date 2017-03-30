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
		/// <param name="fileUrl">Input file.</param>
		public async Task<FaceEmotionDetection> DetectFaceAndEmotionsAsync(string fileUrl)
		{
			throw new NotImplementedException();

			// 1- Obtener las emociones utilizando el servicio EmotionServiceClient (RecognizeAsync)

			// 2- Obtener la lista de rostros utilizando el servicio FaceServiceClient (DetectAsync)
			// 	Enviar la lista completa de attributos FaceAttributeType al servicio (Age,Gender,FacialHair,Smile,HeadPose,Glasses)
		}


		/// <summary>
		/// Crea la persona y le asigna un rostro.
		/// </summary>
		/// <returns></returns>
		/// <param name="name">Name.</param>
		/// <param name="fileUrl">File URL.</param>
		public async Task AddPersonFaceAsync(string name, string fileUrl)
		{
			throw new NotImplementedException();

			// 1- Crear grupo de personas

			// 2- Crear la persona utilizando el servicio FaceServiceClient (CreatePersonAsync)

			// 3- Agregar un rostro a la persona creada utilizando el servicio FaceServiceClient (AddPersonFaceAsync)

			// 4- Entrenar el grupo (TrainPersonGroup)
		}

		/// <summary>
		/// Entrena el algoritmo de identificacion con los rostros del grupo.
		/// </summary>
		/// <returns></returns>
		public async Task TrainPersonGroup()
		{
			throw new NotImplementedException();

			// 1- Entrenar el grupo utilizando el servicio FaceServiceClient (TrainPersonGroupAsync)
		}

		/// <summary>
		/// Identifica los rostros de las personas del grupo en la foto.
		/// </summary>
		/// <returns>Nombre de las personas identificadas</returns>
		/// <param name="fileUrl">File.</param>
		public async Task<List<string>> IdentifyFaceAsync(string fileUrl)
		{
			throw new NotImplementedException();

			// 1- Utilizar el servicio FaceServiceCliente para obtener los FaceId de los rostros en la imagen (DetectAsync)

			// 2- Identificar los rostros del grupo que aparecen en la imagen utilizando el servicio FaceServiceClient (IdentifyAsync)

			// 3- Validar los rostros identificados para ver si hay candidatos contra nuestro grupo

			// 4- Si hay candidatos obtener el detalle utilizando el servicio FaceServiceClient (GetPersonAsync)
		}
	}
}
