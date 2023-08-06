﻿using ExerciseTracker.Models;
using ExerciseTracker.Repositories;
using Microsoft.Identity.Client;
using Spectre.Console;

namespace ExerciseTracker
{
	public class UserInput
	{
		private readonly ExerciseTrackerContext _context;

		public UserInput(ExerciseTrackerContext context)
		{
			_context = context;
		}

		public static ExerciseModel GetExerciseInfo()
		{
			var exercise = new ExerciseModel();
			string type = AnsiConsole.Ask<string>("What exercise did you do ? ").Trim();
			DateTime start = AnsiConsole.Ask<DateTime>("When did the exercise start ? format(yyyy-mm-dd hh:mm): ");
			DateTime end = AnsiConsole.Ask<DateTime>("When did the exercise end ? format (yyyy-mm-dd hh:mm): ");
			string comment = AnsiConsole.Prompt(new TextPrompt<string>("Add a comment about the exercise (optional): ")
				.AllowEmpty());
			

			exercise.ExerciseType = type;
			exercise.DateStart = start;
			exercise.DateEnd = end;
			exercise.Comments = comment;

			return exercise;
		}

		internal static int GetIdOfExercise(List<ExerciseModel> exercises)
		{
			var exerciseArray = exercises.Select(x => $"{x.ExerciseId} - {x.ExerciseType}").ToArray();
			var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
				.Title("Select the desired exercise:")
				.AddChoices(exerciseArray));
			var exerciseId = option.Split(" - ")[0];
			int id = exercises.Single(x => x.ExerciseId.ToString() == exerciseId).ExerciseId;
			return id;
		}

		internal static ExerciseModel GetUpdatedInfo(ExerciseModel exercise)
		{
			exercise.ExerciseType = AnsiConsole.Confirm($"Do you want to update the exercise name ({exercise.ExerciseType}) ?") ?
				AnsiConsole.Ask<string>("Enter a new name: ")
				: exercise.ExerciseType;
			exercise.DateStart = AnsiConsole.Confirm($"Do you want to edit the start time ({exercise.DateStart}) ?") ?
				AnsiConsole.Ask<DateTime>("Enter the new start time: ")
				: exercise.DateStart;
			exercise.DateEnd = AnsiConsole.Confirm($"Do you want to edit the end time ({exercise.DateEnd})?") ?
				AnsiConsole.Ask<DateTime>("Enter the new end time: ")
				:exercise.DateEnd;
			exercise.Comments = AnsiConsole.Confirm($"Do you want to add or change a comment ?") ?
				AnsiConsole.Ask<string>("Enter comment: ")
				:exercise.Comments;

			return exercise;
		}

		public void MainMenu()
		{
			IExerciseRepository exerciseRepository = new ExerciseRepository(_context);
			ExerciseService exerciseService = new ExerciseService(exerciseRepository);
			bool isAppAlive = true;
			while (isAppAlive)
			{
				Console.Clear();
				var option = AnsiConsole.Prompt(new SelectionPrompt<Menu>()
					.Title("What would you like to do?")
					.AddChoices(
						Menu.AddExercise,
						Menu.ViewAllExercises,
						Menu.ViewExercise,
						Menu.UpdateExercise,
						Menu.DeleteExercise,
						Menu.Quit
						));
				switch (option)
				{
					case Menu.AddExercise:
						exerciseService.AddExercise();
						break;
					case Menu.ViewAllExercises:
						exerciseService.GetAll();
						break;
					case Menu.ViewExercise:
						exerciseService.GetExerciseById();
						break;
					case Menu.UpdateExercise: 
						exerciseService.UpdateExercise();
						break;
					case Menu.DeleteExercise:
						exerciseService.DeleteExercise();
						break;
					case Menu.Quit:
						isAppAlive = false;
						break;
				}
			}
		}

		enum Menu
		{
			AddExercise,
			ViewAllExercises,
			ViewExercise,
			UpdateExercise,
			DeleteExercise,
			Quit
		}
	}
}
