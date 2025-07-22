using System.ComponentModel;
using System.Runtime.InteropServices;

namespace denSharedLibrary;

public class XpsPrinter : IXpsPrinter
{
    private string _printerName;

    public void SetPrinter(string printerName)
    {
        _printerName = printerName;
    }

    public void Print(Stream stream)//  string xpsFilePath)
    {
        //   using (Stream stream = new FileStream(xpsFilePath, FileMode.Open, FileAccess.Read))
        // {
        Print(stream, "my job");
        // }
    }

    private void Print(Stream stream, string jobName)
    {
        IntPtr completionEvent = CreateEvent(IntPtr.Zero, true, false, null);
        try
        {
            IXpsPrintJob job;
            IXpsPrintJobStream jobStream;
            StartJob(_printerName, jobName, completionEvent, out job, out jobStream);
            CopyJob(stream, job, jobStream);
            WaitForJob(completionEvent, -1);
            CheckJobStatus(job);
        }
        finally
        {
            if (completionEvent != IntPtr.Zero)
                CloseHandle(completionEvent);
        }
    }

    private void StartJob(string printerName, string jobName, IntPtr completionEvent, out IXpsPrintJob job, out IXpsPrintJobStream jobStream)
    {
        int result = StartXpsPrintJob(printerName, jobName, null, IntPtr.Zero, completionEvent, null, 0, out job, out jobStream, IntPtr.Zero);
        if (result != 0)
            throw new Win32Exception(result);
    }

    private void CopyJob(Stream stream, IXpsPrintJob job, IXpsPrintJobStream jobStream)
    {
        try
        {
            byte[] buff = new byte[4096];
            while (true)
            {
                uint read = (uint)stream.Read(buff, 0, buff.Length);
                if (read == 0)
                    break;
                uint written;
                jobStream.Write(buff, read, out written);

                if (read != written)
                    throw new Exception("Failed to copy data to the print job stream.");
            }
            jobStream.Close();
        }
        catch (Exception)
        {
            job.Cancel();
            throw;
        }
    }

    private void WaitForJob(IntPtr completionEvent, int timeout)
    {
        if (timeout < 0)
            timeout = -1;

        switch (WaitForSingleObject(completionEvent, timeout))
        {
            case WAIT_RESULT.WAIT_OBJECT_0:
                break;

            case WAIT_RESULT.WAIT_TIMEOUT:
                throw new Exception("Timeout expired");
            case WAIT_RESULT.WAIT_FAILED:
                throw new Exception("Wait for the job to complete failed");
            default:
                throw new Exception("Unexpected result when waiting for the print job.");
        }
    }

    private void CheckJobStatus(IXpsPrintJob job)
    {
        XPS_JOB_STATUS jobStatus;
        job.GetJobStatus(out jobStatus);
        switch (jobStatus.completion)
        {
            case XPS_JOB_COMPLETION.XPS_JOB_COMPLETED:
                break;

            case XPS_JOB_COMPLETION.XPS_JOB_IN_PROGRESS:
                break;

            case XPS_JOB_COMPLETION.XPS_JOB_FAILED:
                throw new Win32Exception(jobStatus.jobStatus);
            default:
                throw new Exception("Unexpected print job status.");
        }
    }

    [DllImport("XpsPrint.dll", EntryPoint = "StartXpsPrintJob")]
    private static extern int StartXpsPrintJob(
        [MarshalAs(UnmanagedType.LPWStr)] string printerName,
        [MarshalAs(UnmanagedType.LPWStr)] string jobName,
        [MarshalAs(UnmanagedType.LPWStr)] string outputFileName,
        IntPtr progressEvent,
        IntPtr completionEvent,
        [MarshalAs(UnmanagedType.LPArray)] byte[] printablePagesOn,
        uint printablePagesOnCount,
        out IXpsPrintJob xpsPrintJob,
        out IXpsPrintJobStream documentStream,
        IntPtr printTicketStream);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

    [DllImport("Kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern WAIT_RESULT WaitForSingleObject(IntPtr handle, int milliseconds);

    [DllImport("Kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [Guid("0C733A30-2A1C-11CE-ADE5-00AA0044773D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IXpsPrintJobStream
    {
        void Read([MarshalAs(UnmanagedType.LPArray)] byte[] pv, uint cb, out uint pcbRead);

        void Write([MarshalAs(UnmanagedType.LPArray)] byte[] pv, uint cb, out uint pcbWritten);

        void Close();
    }

    [Guid("5ab89b06-8194-425f-ab3b-d7a96e350161")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IXpsPrintJob
    {
        void Cancel();

        void GetJobStatus(out XPS_JOB_STATUS jobStatus);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XPS_JOB_STATUS
    {
        public uint jobId;
        public int currentDocument;
        public int currentPage;
        public int currentPageTotal;
        public XPS_JOB_COMPLETION completion;
        public int jobStatus;
    }

    private enum XPS_JOB_COMPLETION
    {
        XPS_JOB_IN_PROGRESS = 0,
        XPS_JOB_COMPLETED = 1,
        XPS_JOB_CANCELLED = 2,
        XPS_JOB_FAILED = 3
    }

    private enum WAIT_RESULT
    {
        WAIT_OBJECT_0 = 0,
        WAIT_ABANDONED = 0x80,
        WAIT_TIMEOUT = 0x102,
        WAIT_FAILED = -1
    }
}