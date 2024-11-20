Imports System
Imports System.Runtime.InteropServices

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
    Private Sub chatllm_set_gen_max_tokens(obj As IntPtr, genMaxTokens As Integer)
    End Sub

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Sub chatllm_restart(obj As IntPtr, sysPrompt As String)
    End Sub

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_user_input(obj As IntPtr, input As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_set_ai_prefix(obj As IntPtr, prefix As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_tool_input(obj As IntPtr, input As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_tool_completion(obj As IntPtr, completion As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_text_tokenize(obj As IntPtr, text As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_text_embedding(obj As IntPtr, text As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_qa_rank(obj As IntPtr, question As String, answer As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_rag_select_store(obj As IntPtr, storeName As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Sub chatllm_abort_generation(obj As IntPtr)
    End Sub

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Sub chatllm_show_statistics(obj As IntPtr)
    End Sub

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_save_session(obj As IntPtr, fileName As String) As Integer
    End Function

    <DllImport("libchatllm", CallingConvention:=CallingConvention.StdCall)>
    Private Function chatllm_load_session(obj As IntPtr, fileName As String) As Integer
    End Function

    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Private Delegate Sub PrintCallback(userData As IntPtr, printType As PrintType, utf8Str As String)

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

    Private Sub ChatLLMPrint(userData As IntPtr, printType As PrintType, utf8Str As String)
        Select Case printType
            Case PrintType.PRINT_CHAT_CHUNK
                Console.Write(utf8Str)
            Case Else
                Console.WriteLine(utf8Str)
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

            Console.Write("A.I. > ")
            r = chatllm_user_input(obj, input)
            If r <> 0 Then
                Console.WriteLine(">>> chatllm_user_input error: " & r)
                Exit While
            End If
        End While
    End Sub
End Module
