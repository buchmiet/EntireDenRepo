using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using DataServicesNET80;
using System.Drawing.Imaging;
using DataServicesNET80.Extensions;
using DataServicesNET80.Models;
using DataServicesNET80.DatabaseAccessLayer;

namespace denWebservicesNET80;

[Route("api/[controller]")]
[ApiController]
public class ImageController(IDatabaseAccessLayer databaseAccessLayer) : ControllerBase
{
    // Trasy (Routes) i nazwy akcji
    private const string RemovePicAction = "RemovePic";
    private const string UploadNewPicAction = "UploadNewPic";
    private const string GetThumbnailRoute = "GetThumbnail/{*name}";
    private const string GetThumbnailActionName = "GetThumbnail";

    // Klucze w danych formularza (IFormCollection)
    private const string ItemBodyIdFormField = "itemBodyID";
    private const string FileNameFormField = "fileName";
    private const string PlaceHolderFormField = "placeHolder";

    // Wartości placeholderów
    private const string LogoPicPlaceholder = "logoPic";
    private const string PackagePicPlaceholder = "packagePic";

    // Ścieżki
    private const string BaseProductPicturesPath = @"C:\Inetpub\vhosts\time4parts.co.uk\productpictures\";
    private const string Path800Px = @"C:\Inetpub\vhosts\time4parts.co.uk\productpictures\800px\";
    private const string Path200Px = @"C:\Inetpub\vhosts\time4parts.co.uk\productpictures\200px\";
    private const string Path800PxLqDelete = @"C:\Inetpub\vhosts\time4parts.co.uk\productpictures\800px\lq";
    private const string Path200PxLqDelete = @"C:\Inetpub\vhosts\time4parts.co.uk\productpictures\200px\lq";
    private const string Path800PxLqSave = @"C:\Inetpub\vhosts\time4parts.co.uk\productpictures\800px\lq\";
    private const string Path200PxLqSave = @"C:\Inetpub\vhosts\time4parts.co.uk\productpictures\200px\lq\";

    // Typy plików i MIME
    private const string JpgExtension = ".jpg";
    private const string JpegMimeType = "image/jpeg";

    // Inne stałe
    private const string OkSuccessMessage = "ff";
    private const string OkStatusMessage = "OK";


    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        int j;
        ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
        for (j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }
        return null;
    }

    [HttpPost(RemovePicAction, Name = RemovePicAction)]
    public async Task<IActionResult> RemovePic(IFormCollection formData)
    {
        var errortext = "";
        var files = HttpContext.Request.Form.Files;
        int itemBodyID = Convert.ToInt32(formData[ItemBodyIdFormField]);
        string fileName = formData[FileNameFormField];
        try
        {
            System.IO.File.Delete(BaseProductPicturesPath + fileName);
            System.IO.File.Delete(Path800Px + fileName);
            System.IO.File.Delete(Path800PxLqDelete + fileName);
            System.IO.File.Delete(Path200Px + fileName);
            System.IO.File.Delete(Path200PxLqDelete + fileName);
        }
        catch (Exception ex)
        {
            return Ok(ex.InnerException);
        }
        string akcja = formData[PlaceHolderFormField];

        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var itemBodyService = new EntityService<itembody>(unitOfWork);
        var photosService = new EntityService<photo>(unitOfWork);
        var body = await itemBodyService.GetOneAsync(p => p.itembodyID == itemBodyID);

        if (akcja.Equals(LogoPicPlaceholder))
        {
            body.logoPic = null;
            await itemBodyService.UpdateAsync(body);
            return Ok();
        }
        if (akcja.Equals(PackagePicPlaceholder))
        {
            body.packagePic = null;
            await itemBodyService.UpdateAsync(body);

            return Ok();
        }
        try
        {
            var fotka = await photosService.GetOneAsync(p => p.path.Equals(fileName));
            await photosService.DeleteAsync(fotka);


            var remainingPhotos = (await photosService.GetAllAsync(p => p.itembodyID == itemBodyID)).OrderBy(p => p.pos).ToList();
            for (int i = 0; i < remainingPhotos.Count; i++)
            {
                remainingPhotos[i].pos = i;
            }
            await photosService.UpdateRangeAsync(remainingPhotos);
        }
        catch (Exception ex)
        {
            return Ok(ex.InnerException);
        }
        return Ok(OkSuccessMessage);
    }



    [HttpPost(UploadNewPicAction, Name = UploadNewPicAction)]
    public async Task<IActionResult> UploadNewPic(IFormCollection formData)
    {
        var files = HttpContext.Request.Form.Files;

        var g = Guid.NewGuid();
        while (System.IO.File.Exists(BaseProductPicturesPath + g + JpgExtension)) { g = Guid.NewGuid(); }
        var file1 = files[0];
        MemoryStream ms = new MemoryStream();
        await file1.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);

        await using (var fileStream = new FileStream(BaseProductPicturesPath + g + JpgExtension, FileMode.Create))
        {
            await ms.CopyToAsync(fileStream);
        }
        ms.Seek(0, SeekOrigin.Begin);
        var bmp = new Bitmap(ms);
        double szer = bmp.Width;
        double wys = bmp.Height;
        var nowaWys = szer / 800;
        nowaWys = wys / nowaWys;
        var newBmp = new Bitmap(bmp, new Size(800, Convert.ToInt32(nowaWys)));
        var myEncoderParameters = new EncoderParameters(1);
        var myImageCodecInfo = GetEncoderInfo(JpegMimeType);
        var myEncoder = System.Drawing.Imaging.Encoder.Quality;
        var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        newBmp.Save(Path800Px + g + JpgExtension, myImageCodecInfo, myEncoderParameters);
        myEncoderParameter = new EncoderParameter(myEncoder, 20L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        newBmp.Save(Path800PxLqSave + g + JpgExtension, myImageCodecInfo, myEncoderParameters);
        newBmp = new Bitmap(bmp, new Size(200, Convert.ToInt32(nowaWys / 4)));
        myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        newBmp.Save(Path200Px + g + JpgExtension, myImageCodecInfo, myEncoderParameters);
        myEncoderParameter = new EncoderParameter(myEncoder, 20L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        newBmp.Save(Path200PxLqSave + g + JpgExtension, myImageCodecInfo, myEncoderParameters);

        int itemBodyID = Convert.ToInt32(formData[ItemBodyIdFormField]);


        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var itemBodyService = new EntityService<itembody>(unitOfWork);
        var photosService = new EntityService<photo>(unitOfWork);
        var body = await itemBodyService.GetOneAsync(p => p.itembodyID == itemBodyID);


        string akcja = formData[PlaceHolderFormField];
        if (akcja.Equals(LogoPicPlaceholder))
        {
            body.logoPic = g + JpgExtension;
            await itemBodyService.UpdateAsync(body);
            return Ok(body.logoPic);
        }

        if (akcja.Equals(PackagePicPlaceholder))
        {
            body.packagePic = g + JpgExtension;
            await itemBodyService.UpdateAsync(body);
            return Ok(body.packagePic);
        }

        var fotki = (await photosService.GetAllAsync(peh => peh.itembodyID == body.itembodyID)).Count();
        var ph = new photo
        {
            itembodyID = body.itembodyID,
            path = g + JpgExtension,
            pos = fotki
        };
        await photosService.AddAsync(ph);

        return Ok(g + JpgExtension);
    }


    [HttpGet(GetThumbnailRoute, Name = GetThumbnailActionName)]

    public IActionResult GetThumbnail(string name)
    {
        string kod = OkStatusMessage;
        MemoryStream ImageStream = new MemoryStream();
        string ss = "";
        try
        {
            ss = name + JpgExtension;
            var xx = BaseProductPicturesPath + ss;
            System.Drawing.Bitmap bmp2 = new Bitmap(xx);
            Bitmap resized = new Bitmap(bmp2, new Size(bmp2.Width / 4, bmp2.Height / 4));
            resized.Save(ImageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            kod = xx;
        }
        catch (Exception ex)
        {
            kod = ex.Message;
        }
        ImageStream.Seek(0, SeekOrigin.Begin);
        return File(ImageStream.ToArray(), JpegMimeType, name + JpgExtension);
    }


}