Imports System
Imports System.Runtime.InteropServices
Imports System.Text

Module ChatLLM
    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_create() As IntPtr
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Sub chatllm_append_param(obj As IntPtr, param As String)
    End Sub

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_start(obj As IntPtr, printCallback As PrintCallback, endCallback As EndCallback, userData As IntPtr) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Sub chatllm_user_input(obj As IntPtr, input As IntPtr)
    End Sub

    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Private Delegate Sub PrintCallback(userData As IntPtr, printType As PrintType, utf8Str As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Private Delegate Sub EndCallback(userData As IntPtr)

    Private Enum PrintType
        PRINT_CHAT_CHUNK = 0
        PRINTLN_META = 1
        PRINTLN_ERROR = 2
        PRINTLN_REF = 3
        PRINTLN_REWRITTEN_QUERY = 4
        PRINTLN_HISTORY_USER = 5
        PRINTLN_HISTORY_AI = 6
        PRINTLN_TOOL_CALLING = 7
        PRINTLN_EMBEDDING = 8
        PRINTLN_RANKING = 9
        PRINTLN_TOKEN_IDS = 10
    End Enum

    Private Sub ChatLLMPrint(userData As IntPtr, printType As PrintType, utf8Str As IntPtr)
        ' Read the UTF-8 bytes from the pointer
        Dim utf8Bytes As List(Of Byte) = New List(Of Byte)
        Dim currentByte As Byte
        Dim ptr As IntPtr = utf8Str

        Do
            currentByte = Marshal.ReadByte(ptr)
            If currentByte = 0 Then
                Exit Do
            End If
            utf8Bytes.Add(currentByte)
            ptr = IntPtr.Add(ptr, 1)
        Loop

        ' Convert the UTF-8 bytes to a UTF-16 string
        Dim utf16String As String = Encoding.UTF8.GetString(utf8Bytes.ToArray())

        Select Case printType
            Case PrintType.PRINT_CHAT_CHUNK
                Console.Write(utf16String)
            Case Else
                Console.WriteLine(utf16String)
        End Select
    End Sub

    Private Sub ChatLLMEnd(userData As IntPtr)
        Console.WriteLine()
    End Sub

    Sub Main(args As String())
        Dim obj As IntPtr = chatllm_create()
        For Each arg As String In args
            chatllm_append_param(obj, arg)
        Next

        Dim r As Integer = chatllm_start(obj, AddressOf ChatLLMPrint, AddressOf ChatLLMEnd, IntPtr.Zero)
        If r <> 0 Then
            Console.WriteLine(">>> chatllm_start error: " & r)
            Return
        End If

        While True
            Console.Write("You  > ")
            Dim input As String = Console.ReadLine()
            If String.IsNullOrEmpty(input) Then Continue While

            ' Convert the input string to UTF-8
            Dim utf8Bytes As Byte() = Encoding.UTF8.GetBytes(input & vbNullChar) ' Add null terminator
            Dim utf8Ptr As IntPtr = Marshal.AllocHGlobal(utf8Bytes.Length)
            Marshal.Copy(utf8Bytes, 0, utf8Ptr, utf8Bytes.Length)

            Console.Write("A.I. > ")
            chatllm_user_input(obj, utf8Ptr)

            ' Free the allocated memory
            Marshal.FreeHGlobal(utf8Ptr)
        End While
    End Sub
End Module
