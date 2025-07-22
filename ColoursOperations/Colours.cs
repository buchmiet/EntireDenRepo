using DataServicesNET80.Models;

namespace ColoursOperations;

public interface IColoursService
{
    void SetMediator(IColourOpsMediator mediator);
    byte[] ReplaceColours(byte[] obrazek, Dictionary<int, int> translacje);
    List<byte[]> FindColours(byte[] bsource);
    Task<KeyValuePair<int, object>> GetOneColour(colourtranslation translation);
    void RaiseColourEvent(int id);
    Task<object> Translation2Source(colourtranslation kolek);
    event ColoursService.DataChangedEventHandler ColourChanged;
}

public class ColoursService : IColoursService
{

    private IColourOpsMediator _mediator;

    public void SetMediator(IColourOpsMediator mediator)
    {
        _mediator = mediator;
    }

    public byte[] ReplaceColours(byte[] obrazek, Dictionary<int, int> translacje)
    {

        var zwrotka = new byte[obrazek.Length];
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                var buf = new byte[4];
                try
                {
                    Buffer.BlockCopy(obrazek, ((y * 128) + x) * 4, buf, 0, 4);
                }
                catch (Exception e)
                {
                    var ty = e.Message;
                }
                int result1 = BitConverter.ToInt32(buf, 0);
                var heksik = result1.ToString("X8");
                if (translacje.ContainsKey(result1))
                {
                    byte[] nowykol = BitConverter.GetBytes(translacje[result1]);
                    var nowykol2 = new byte[4];
                    nowykol2[0] = nowykol[3];
                    nowykol2[1] = nowykol[2];
                    nowykol2[2] = nowykol[1];
                    nowykol2[3] = nowykol[0];
                    Buffer.BlockCopy(nowykol, 0, zwrotka, ((y * 128) + x) * 4, 4);
                }
                else
                {
                    Buffer.BlockCopy(buf, 0, zwrotka, ((y * 128) + x) * 4, 4);
                }
            }
        }
        return zwrotka;
    }

    public List<byte[]> FindColours(byte[] bsource)
    {
        var zwrotka = new List<byte[]>();
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                var buf = new byte[4];
                try
                {
                    Buffer.BlockCopy(bsource, ((y * 128) + x) * 4, buf, 0, 4);
                }
                catch (Exception e)
                {
                    var tre = e.Message;
                }

                bool jest = false;
                foreach (var bufek in zwrotka)
                {
                    if (bufek[0] == buf[0] &&
                        bufek[1] == buf[1] &&
                        bufek[2] == buf[2] &&
                        bufek[3] == buf[3]
                       )
                    {
                        jest = true;
                    }
                }

                if (!jest)
                {
                    zwrotka.Add(buf);
                }
            }
        }
        return zwrotka.ToList();
    }



    public async Task<KeyValuePair<int, object>> GetOneColour(colourtranslation translation)
    {
        colourtranslation kolek;
        byte[] pe = (await Translation2Source(translation).ConfigureAwait(false)) as byte[];
        var zwrotka = _mediator.ConvertToPlatformBitmap(pe);
        return new KeyValuePair<int, object>(translation.kodKoloru, zwrotka);
    }

    public void RaiseColourEvent(int id)
    {
        ColourChanged?.Invoke(id);
    }



    public async Task<object> Translation2Source(colourtranslation kolek)
    {
        var path = Path.Combine(AppContext.BaseDirectory, @"Data\" + kolek.schemat);

        var bufor = await _mediator.GetPixelsFromImageAsync(path).ConfigureAwait(false);

        var starekolory = FindColours(bufor).ToArray();

        var tempik = new Dictionary<int, int>();
        tempik.Add(BitConverter.ToInt32(starekolory[0], 0), kolek.col1);


        if (kolek.col2 != null)
        {
            tempik.Add(BitConverter.ToInt32(starekolory[1], 0), (int)kolek.col2);

        }
        if (kolek.col3 != null)
        {
            tempik.Add(BitConverter.ToInt32(starekolory[2], 0), (int)kolek.col3);
        }
        if (kolek.col4 != null)
        {

            tempik.Add(BitConverter.ToInt32(starekolory[3], 0), (int)kolek.col3);
        }

        bufor = ReplaceColours(bufor, tempik);
        return bufor;


    }

    public delegate void DataChangedEventHandler(int changedColour);
    public event DataChangedEventHandler ColourChanged;

}