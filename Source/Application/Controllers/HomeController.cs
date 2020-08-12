using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Application.Controllers
{
	public class HomeController : Controller
	{
		#region Methods

		[HttpPost]
		public virtual async Task<IActionResult> Convert(IFormFile file)
		{
			if(file == null)
				throw new ArgumentNullException(nameof(file));

			// ReSharper disable ConvertToUsingDeclaration
			await using(var stream = file.OpenReadStream())
			{
				var configurationBuilder = new ConfigurationBuilder();
				configurationBuilder.AddJsonStream(stream);
				var configuration = configurationBuilder.Build();
				var settings = new List<Setting>();
				this.PopulateSettings(configuration, settings);

				return await Task.FromResult(this.Json(settings));
			}
			// ReSharper restore ConvertToUsingDeclaration
		}

		public virtual async Task<IActionResult> Index()
		{
			return await Task.FromResult(this.View());
		}

		protected internal virtual void PopulateSettings(IConfiguration configuration, IList<Setting> settings)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if(settings == null)
				throw new ArgumentNullException(nameof(settings));

			if(configuration is IConfigurationSection configurationSection && configurationSection.Value != null)
			{
				settings.Add(new Setting
				{
					Name = configurationSection.Path.Replace(":", "__", StringComparison.Ordinal),
					Value = configurationSection.Value
				});
			}
			else
			{
				foreach(var child in configuration.GetChildren())
				{
					this.PopulateSettings(child, settings);
				}
			}
		}

		#endregion
	}
}