using LessonMigration.Data;
using LessonMigration.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LessonMigration.Utilities.File;
using LessonMigration.Utilities.Helpers;

namespace LessonMigration.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
        public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
      
        public async Task <IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders.AsNoTracking().ToListAsync();
            return View(sliders);
        }
     
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task < IActionResult>Create(Slider slider)
        {
            if (ModelState["Photo"].ValidationState == ModelValidationState.Invalid) return View(); //bu photo mutleg secmelsen demekdi

            /* if (!slider.Photo.ContentType.Contains("image/"))*/ // image tipini check edirik 

            if (!slider.Photo.CheckFileType ("image/")) //optimize hali 
            {
                ModelState.AddModelError("Photo", "Image type is wrong");
                return View();
            }

           /* if (slider.Photo.Length / 1024 > 200)*/ // image size check edirik
              
              if (!slider.Photo.CheckFileSize(300)) // optimize halda size check edirik 
            {
                ModelState.AddModelError("Photo", "Image size is wrong");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + " " + slider.Photo.FileName; //her defe weklin adi ferqli gelsin deye

            /* string path = Path.Combine(_env.WebRootPath, "img", fileName);*/  //rootu dynamic elemek ucundu
            string path = Helper.GetFilePath(_env.WebRootPath, "img", fileName); //optimize halidi


            using (FileStream stream = new FileStream (path, FileMode.Create)) //using yaziriqsa qebz collectory isleyecek,ve bu code blockdan sonra cashdan silinecek derhal
            {
               await slider.Photo.CopyToAsync(stream);
            }

            slider.Image = fileName;
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Delete(int id)
        {
            //Slider slider = await _context.Sliders.FindAsync(id); ilk hali 
            Slider slider = await GetSliderById(id); //optimize hali 102 line de yazlib 

            if (slider == null) return NotFound();
            //string path = Path.Combine(_env.WebRootPath, "img", slider.Image); 
            string path = Helper.GetFilePath(_env.WebRootPath, "img", slider.Image); //optimize hali

            //if (System.IO.File.Exists(path))
            //{                                         //ilk hali
            //    System.IO.File.Delete(path);
            //}

            Helper.DeleteFile(path); //optimize hali

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private async Task <Slider> GetSliderById(int id)
        {
            return await _context.Sliders.FirstAsync();
        }


        public async Task<IActionResult> Update(int id)
        {
            Slider slider = await _context.Sliders.FindAsync(id);

            if (slider == null) NotFound();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Slider slider)
        {
            Slider dbSlider = await GetSliderById(id);

            string path = Helper.GetFilePath(_env.WebRootPath, "img", dbSlider.Image);

            Helper.DeleteFile(path);

            if (!slider.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "Image type is wrong");
                return View();
            }

            if (!slider.Photo.CheckFileSize(300))
            {
                ModelState.AddModelError("Photo", "image size is wrong");
                return View();
            }

            string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;

            string pathNew = Helper.GetFilePath(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new FileStream(pathNew, FileMode.Create))
            {
                await slider.Photo.CopyToAsync(stream);
            }

            dbSlider.Image = fileName;
            await _context.SaveChangesAsync();

            return RedirectToAction();
        }
    }
}
