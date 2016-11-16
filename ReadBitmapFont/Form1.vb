Imports System
Imports System.IO
Imports System.Drawing.Imaging
Imports Microsoft.Win32
Imports System.Security
Imports System.Security.Permissions

Public Class Form1

    Public Structure FontBinHeader
        Dim Version As UShort          'Version
        Dim FontType As UShort         'FontType
        Dim FileTotalSize As Integer    'FileTotalSize
        Dim FontHeaderSize As Byte   'FontHeaderSize
        Dim FontDelta As Byte
        Dim PixWidth As UShort
        Dim PixHeight As UShort
        Dim WidthByte As UShort
        Dim FirstChar As UShort
        Dim DefaultChar As UShort
        Dim CodeTableCount As UShort
        Dim Rel2 As UShort
        Dim Rel3 As Integer
        Dim Rel4 As Integer

        'Dim CharSize As Integer

        'Function CalcCharSize(ByVal Rate As Single) As Integer
        '    'CharSize計算
        '    CharSize = WidthByte * PixHeight
        '    CalcCharSize = CharSize
        'End Function


    End Structure
    Dim binHeader As FontBinHeader
    Public Structure AFontHeader
        Dim lFontMark As Integer
        Dim lFontSize As Integer
        Dim lFontImgOffs As Integer
        Dim lFontId As UShort
        Dim bFontCode As Byte
        Dim bFontBarFlag As Byte
        Dim bFontStartCode As Byte
        Dim bFontEndCode As Byte
        Dim bFontSubGroup As Byte
        Dim bFontZeroType As Byte
        Dim bFontFlashFont As Byte
        Dim sFontOther0 As Byte
        Dim sFontOther1 As Byte
        Dim sFontOther2 As Byte
        Dim sFontOther3 As Byte
        Dim sFontOther4 As Byte
        Dim sFontOther5 As Byte
        Dim sFontOther6 As Byte
        Dim sFontOther7 As Byte
        Dim sFontOther8 As Byte
        Dim sFontOther9 As Byte
        Dim sFontOther10 As Byte

        Dim lFontFd_Size As UShort
        Dim bFontValu2 As Byte
        Dim bFont_Type As Byte
        Dim lFontValu4 As UShort
        Dim lFontBase_Line As UShort
        Dim lFontCell_W As UShort
        Dim lFontCell_H As UShort
        Dim bFontOrient As Byte
        Dim bFontSpace As Byte
        Dim lFontSymb_Set As UShort
        Dim lFontPitch As UShort
        Dim lFontHeight As UShort
        Dim lFontValu20 As UShort
        Dim bFontValu22 As Byte
        Dim bFontStyle As Byte
        Dim bFontStroke As Byte
        Dim bFontTypefc As Byte
    End Structure
    Dim binAFontHeader As AFontHeader
    Dim sFilePath As String

    Public Sub Write_Registry(ByVal sSegment As String, ByVal sRegName As String, ByVal sRegValue As Object)
        '把資料寫入註冊區
        Dim rRoot As RegistryKey
        Dim rReg As RegistryKey
        'CreateRegistry()

        rRoot = Registry.CurrentUser.OpenSubKey("Software\KCW\ReadBitmapFont", True)
        rReg = IIf(sSegment = "", rRoot, rRoot.OpenSubKey(sSegment, True))
        rReg.SetValue(sRegName, sRegValue)

        rReg.Close()
        rRoot.Close()
    End Sub

    Private Sub CreateRegistry()
        '       Dim rRoot As RegistryKey

        'rRoot = Registry.LocalMachine.OpenSubKey("Software", True)
        ' Try
        'rRoot = Registry.LocalMachine.OpenSubKey("Software", True)
        My.Computer.Registry.CurrentUser.CreateSubKey("Software\KCW\ReadBitmapFont")
        My.Computer.Registry.CurrentUser.CreateSubKey("Software\KCW\ReadBitmapFont\PARA")
        'My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\ReadBitmapFont", "FileRoot", "C:\", RegistryValueKind.String)
        '        rRoot = Registry.CurrentUser.OpenSubKey("Software", True).CreateSubKey("KCW")
        'rRoot = rRoot.CreateSubKey("KCW")
        '        rRoot = rRoot.CreateSubKey("READ_BITMAP_FONT")
        'rRoot = rRoot.CreateSubKey("READ_BITMAP_FONT")

        '       rRoot = rRoot.OpenSubKey("READ_BITMAP_FONT", True)
        '        rRoot = rRoot.CreateSubKey("PARA")
        '       rRoot.Close()
        'Catch e As SecurityException
        'MessageBox.Show(e.Message, "Security Exception")
        'Catch
        '        MessageBox.Show("CreateRegistry error!", "Read Error")
        'End Try

    End Sub

    Private Sub ReadRegistry()
        Dim rRoot As RegistryKey

        rRoot = Registry.CurrentUser.OpenSubKey("Software\KCW\ReadBitmapFont\PARA", False)
        sFilePath = rRoot.GetValue("FileRoot", "C:\")
        rRoot.Close()
    End Sub
    'return 2 for SB-AFont; 1 for SB-ZFont
    Function CheckFontHeaderType(ByVal fstUS As UShort, ByVal senUS As UShort) As Byte
        If fstUS <= 0 Then
            Return 0
        End If
        If senUS > 0 Then
            Return senUS
        End If
        Return 0
    End Function
    Function OpenRead(ByVal pathStr As String) As Boolean

        Dim file_root, file_name, file_ext As String
        Dim binType As Byte

        If pathStr = "" Then
            MessageBox.Show("Empty Path!")
            Return False
        End If
        file_ext = GetFileExtFromPath(pathStr)
        If file_ext = "" Then
            MessageBox.Show("ExtNameErr!")
            Return False
        Else
            file_root = GetFileRootFromPath(pathStr)
            file_name = GetFilenameFromPath(pathStr)
        End If

        'open file
        Try
            Using fs As FileStream = File.Open(pathStr, FileMode.Open)
                binHeader.Version = Read2ByteInv(fs)
                binHeader.FontType = Read2ByteInv(fs)
                binType = CheckFontHeaderType(binHeader.Version, binHeader.FontType)

                If (binType > 0) Then
                    binHeader.FileTotalSize = Read4ByteInv(fs)
                    binHeader.FontHeaderSize = fs.ReadByte()

                    binHeader.FontDelta = fs.ReadByte()
                    binHeader.PixWidth = Read2ByteInv(fs)
                    binHeader.PixHeight = Read2ByteInv(fs)
                    binHeader.WidthByte = Read2ByteInv(fs)
                    binHeader.FirstChar = Read2ByteInv(fs)
                    binHeader.DefaultChar = Read2ByteInv(fs)
                    binHeader.CodeTableCount = Read2ByteInv(fs)
                    binHeader.Rel2 = Read2ByteInv(fs)
                    binHeader.Rel3 = Read4ByteInv(fs)
                    binHeader.Rel4 = Read4ByteInv(fs)
                End If
                fs.Close()
            End Using
        Catch IOExcep As System.IO.IOException
            MessageBox.Show("The process cannot access the file because it is being used by another process!", "Read Error") '由於另一個處理序正在使用檔案
            Return False
        End Try


        VersionText.Text = binHeader.Version
        FontTypeText.Text = binHeader.FontType
        FileTotalSizeText.Text = binHeader.FileTotalSize
        FontHeaderSizeText.Text = binHeader.FontHeaderSize
        FontDeltaText.Text = binHeader.FontDelta
        Dim str1 As String = String.Format("{0}x{1}", binHeader.PixWidth, binHeader.PixHeight)
        PixWHText.Text = str1
        WidthByteText.Text = binHeader.WidthByte
        FirstCharText.Text = binHeader.FirstChar
        DefaultCharText.Text = binHeader.DefaultChar
        CodeTableCountText.Text = binHeader.CodeTableCount
        Write_Registry("PARA", "FileRoot", pathStr)
        Return True
    End Function
    ' 顯示所有字
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim pathStr As String
        Dim ret As Boolean = False
        Dim charOffset As UInteger
        Dim fileOffset As UInteger

        'open, and read header
        pathStr = ""
        pathStr = FilePath.Text.ToString()
        ret = OpenRead(pathStr)
        If ret = False Then
            Return
        End If
        If ShowCharText.Text = "" Then
            ShowCharText.Text = binHeader.FirstChar
        End If


        Dim g As Graphics = PictureBox1.CreateGraphics()
        ' 將畫布 g 清為白色
        g.Clear(ColorTranslator.FromOle(QBColor(15)))

        'read Code Table 
        Dim codeTable(binHeader.CodeTableCount) As UInteger
        Dim charData(binHeader.PixHeight * binHeader.WidthByte) As Byte
        Dim charBinary(binHeader.PixHeight - 1, binHeader.PixWidth - 1) As Byte
        Dim curX, curY As Integer
        Dim space_size As Integer
        Dim space_line As Integer
        Dim TW, TH As Integer
        Dim image1 As Bitmap

        curX = 0
        curY = 0
        space_size = 10
        space_line = 5

        TW = (binHeader.PixWidth + space_size) * 16
        TH = (binHeader.PixHeight + space_line) * 16
        'If PictureBox1.Width < TW Then
        '    PictureBox1.Width = TW + 10
        'End If
        'If PictureBox1.Height < TH Then
        '    PictureBox1.Height = TH + 10
        'End If
        'PictureBox1.Refresh()

        image1 = New Bitmap(TW, TH)
        fileOffset = 0
        Using fs As FileStream = File.Open(pathStr, FileMode.Open)
            'Get offset value
            ReadByteFlow(fs, binHeader.FontHeaderSize)
            'read code table
            For i As Integer = 0 To binHeader.CodeTableCount - 1 Step 1
                charOffset = Read4ByteInv(fs)
                codeTable(i) = charOffset
            Next

            For i As Integer = 0 To binHeader.CodeTableCount - 1 Step 1

                If i <> 0 And i Mod 16 = 0 Then
                    curX = 0
                    curY = curY + binHeader.PixHeight + space_line
                End If
                'get char offset
                charOffset = codeTable(i)
                'check char offset
                If charOffset <> 4294967295 Then
                    'has this char, show it.
                    If charOffset > fileOffset Then
                        fileOffset = charOffset ' update fileOffset
                        ret = ReadByteArray(fs, binHeader.PixHeight * binHeader.WidthByte, charData)
                        convert2BinArr(binHeader.PixHeight * binHeader.WidthByte, charData, charBinary)
                        For y = 0 To binHeader.PixHeight - 1
                            For x = 0 To binHeader.PixWidth - 1
                                If charBinary(y, x) = 1 Then
                                    'g.FillRectangle(Brushes.Red, x + curX, y + curY, 1, 1)
                                    image1.SetPixel(x + curX, y + curY, Color.Black)
                                End If
                            Next
                        Next
                    'Else
                        ' 重複的字
                    End If
                End If

                curX = curX + binHeader.PixWidth + space_size

            Next
            fs.Close()
        End Using

        ' Set the PictureBox to display the image.
        PictureBox1.Image = image1

        'Show to PictureBox1
        'Dim g As Graphics = PictureBox1.CreateGraphics()
        'Dim LPen As New System.Drawing.Pen(System.Drawing.Color.Black)
        'Dim b As New SolidBrush(Color.Black)

        '' 將畫布 g 清為白色
        'g.Clear(ColorTranslator.FromOle(QBColor(15)))

        ''LPen.DashStyle = Drawing2D.DashStyle.Dot
        ''g.DrawLine(LPen, 1, 25, 2, 25)
        ''g.DrawString("XXX", New Font("Arial", 44, FontStyle.Bold), Brushes.Blue, 20, 20)
        'For y = 0 To binHeader.PixHeight - 1
        '    For x = 0 To binHeader.PixWidth - 1
        '        If PixelDataArray(y, x) = 1 Then
        '            g.FillRectangle(Brushes.Red, x, y, 1, 1)
        '        End If
        '    Next
        'Next
    End Sub
    Function GetFilenameFromPath(ByVal strPath As String) As String
        ' Returns the rightmost characters of a string upto but not including the rightmost '\'
        ' e.g. 'c:\winnt\win.ini' returns 'win.ini'
        GetFilenameFromPath = ""
        If Microsoft.VisualBasic.Right$(strPath, 1) <> "\" And Len(strPath) > 0 Then
            GetFilenameFromPath = GetFilenameFromPath(Microsoft.VisualBasic.Left$(strPath, Len(strPath) - 1)) + Microsoft.VisualBasic.Right$(strPath, 1)
        End If
    End Function
    Function GetFileNameFromPathWithoutExt(ByVal strPath As String) As String
        ' Returns the rightmost characters of a string upto but not including the rightmost '\'
        ' e.g. 'c:\winnt\win.ini' returns 'win.ini'
        Dim strName As String = ""
        Dim strExt As String = ""

        GetFileNameFromPathWithoutExt = ""
        strName = GetFilenameFromPath(strPath) ' file name
        strExt = GetFileExtFromPath(strPath) ' file Ext
        If Microsoft.VisualBasic.Right$(strName, Len(strExt)) = strExt And Len(strName) > 0 Then
            GetFileNameFromPathWithoutExt = Microsoft.VisualBasic.Left$(strName, Len(strName) - Len(strExt))
            'GetFileNameFromPathWithoutExt = GetFilenameFromPath(Microsoft.VisualBasic.Left$(strPath, Len(strPath) - 1)) + Microsoft.VisualBasic.Right$(strPath, 1)
        End If
    End Function
    Function GetFileRootFromPath(ByVal strPath As String) As String
        ' Returns the rightmost characters of a string upto but not including the rightmost '\'
        ' e.g. 'c:\winnt\win.ini' returns 'c:\winnt'
        Dim strT As String

        GetFileRootFromPath = ""
        strT = GetFilenameFromPath(strPath) ' file name
        If Microsoft.VisualBasic.Right$(strPath, Len(strT)) = strT And Len(strPath) > 0 Then
            GetFileRootFromPath = Microsoft.VisualBasic.Left$(strPath, Len(strPath) - Len(strT))
        End If
    End Function
    Function GetFileExtFromPath(ByVal strPath As String) As String
        ' Returns the rightmost characters of a string upto but not including the rightmost '\'
        ' e.g. 'c:\winnt\win.ini' returns 'ini'

        GetFileExtFromPath = ""
        If Microsoft.VisualBasic.Right$(strPath, 1) <> "." And Len(strPath) > 0 Then
            GetFileExtFromPath = GetFileExtFromPath(Microsoft.VisualBasic.Left$(strPath, Len(strPath) - 1)) + Microsoft.VisualBasic.Right$(strPath, 1)
        End If
    End Function
    Function Read2ByteInv(ByVal fs As FileStream) As UShort
        Dim LoByte As Byte
        Dim HiByte As Byte

        LoByte = fs.ReadByte()
        HiByte = fs.ReadByte()
        Read2ByteInv = (HiByte * 256) + LoByte
    End Function
    Function Read4ByteInv(ByVal fs As FileStream) As UInteger
        Dim LoLoByte, LoHiByte As Byte
        Dim HiLoByte, HiHiByte As Byte
        Dim i, t As UInt64

        LoLoByte = fs.ReadByte()
        LoHiByte = fs.ReadByte()
        HiLoByte = fs.ReadByte()
        HiHiByte = fs.ReadByte()
        t = HiHiByte
        i = (t * 16777216)
        i = i + (HiLoByte * 65536)
        i = i + (LoHiByte * 256)
        i = i + LoLoByte
        Read4ByteInv = i
    End Function
    Function ReadByteFlow(ByVal fs As FileStream, ByVal n As Integer) As Boolean
        For i As Integer = 1 To n Step 1
            fs.ReadByte()
        Next i
        ReadByteFlow = True
    End Function
    Function ReadByteArray(ByVal fs As FileStream, ByVal n As Integer, ByRef arr() As Byte) As Boolean
        Dim i As Integer
        For i = 0 To n - 1 Step 1
            arr(i) = fs.ReadByte()
        Next i
        ReadByteArray = True
    End Function

    Function WriteByteArray(ByVal fs As FileStream, ByVal n As Integer, ByRef arr() As Byte) As Boolean
        Dim i As Integer
        For i = 0 To n - 1 Step 1
            fs.WriteByte(arr(i))
        Next i
        WriteByteArray = True
    End Function
    Function IsHexLeader(ByVal s As String) As Boolean
        If s.Length < 3 Then
            IsHexLeader = False
            Return IsHexLeader
        End If
        If s(0) = "0" And (s(1) = "x" Or s(1) = "X") Then
            IsHexLeader = True
            Return IsHexLeader
        End If
        IsHexLeader = False
        Return IsHexLeader
    End Function
    Function IsHex(ByVal s As String) As Boolean
        Dim strlen As Integer = s.Length
        Dim i As Integer

        If IsHexLeader(s) = False Then
            IsHex = False
            Return IsHex
        End If

        For i = 2 To strlen - 1
            If IsNumeric(s(i)) Then
                Continue For
                'ElseIf Val(Convert.ToChar(s(i))) >= 65 And Val(Convert.ToChar(s(i))) <= 70 Then
            ElseIf Val(s(i)) >= 65 And Val(s(i)) <= 70 Then
                Continue For
            Else
                IsHex = False
                Return IsHex
            End If
        Next

        IsHex = True
        Return IsHex
    End Function
    Function HexStr2Int(ByVal s As String, ByVal sh As Integer) As Integer
        Dim strlen As Integer = s.Length
        Dim i, tmp, ret As Integer
        If strlen = 0 Or sh > strlen Then
            'HexStr2Int = 0
            Return 0
        End If
        ret = 0
        For i = sh To strlen - 1
            If IsNumeric(s(i)) Then
                tmp = Convert.ToInt32(s(i)) - 48
            Else
                tmp = Val(Convert.ToChar(s(i))) + 9 - 64
            End If


            ret = ret * 16 + tmp
        Next
        HexStr2Int = ret
        Return HexStr2Int
    End Function
    Function GetCharOffset(ByVal fs As FileStream, ByVal pathStr As String, ByVal CodeTableOffset As Integer, ByVal showNoChar As Boolean) As UInteger
        Dim ret As UInteger
        ret = 0
        'Get offset value
        ReadByteFlow(fs, CodeTableOffset)
        ret = Read4ByteInv(fs)
        'check offset value, if 0xFFFF FFFF
        If ret >= 4294967295 Then
            If (showNoChar = True) Then
                MessageBox.Show("No this char!", "Code Table Err")
            End If
            ret = 0
            Return 0
        End If
        'check offset value, if over BinSize
        If ret >= binHeader.FileTotalSize Then
            MessageBox.Show("Code Table Err! Over file size.", "Code Table Err")
            ret = 0
            Return 0
        End If

        GetCharOffset = ret
        Return GetCharOffset
    End Function
    Function convert2BinArr(ByVal CharSize As Integer, ByVal CharDataArray() As Byte, ByVal PixelDataArray(,) As Byte) As Boolean
        ' convert to binary for pixel, integer to binary
        'Dim PixelDataArray(binHeader.PixHeight - 1, binHeader.PixWidth - 1) As Byte
        Dim binaryStr As String
        Dim i, stop1 As Integer
        Dim traceStr As String = "", traceStr2 As String
        Dim x, y, row, col As Integer
        x = 0
        y = 0
        row = 0
        col = 0
        For index = 0 To CharSize - 1 Step 1

            binaryStr = Convert.ToString(CharDataArray(index), 2)
            If binaryStr.Length < 8 Then
                'add '0'
                For i = 1 To 8 - binaryStr.Length
                    binaryStr = "0" + binaryStr
                Next
            End If
            If (x + 8) < binHeader.PixWidth Then
                stop1 = 7
            Else
                stop1 = binHeader.PixWidth - x - 1
            End If

            For i = 0 To stop1 Step 1
                traceStr2 = Mid(binaryStr, i + 1, 1)
                PixelDataArray(col, x) = traceStr2
                If traceStr.Length > 0 Then
                    traceStr = traceStr + traceStr2
                    'If traceStr2 = "0" Then
                    '    traceStr = traceStr + "0"
                    'ElseIf traceStr2 = "1" Then
                    '    traceStr = traceStr + "@"
                    'End If
                Else
                    traceStr = traceStr2
                End If
                x = x + 1
            Next
            If x >= binHeader.PixWidth Then
                x = 0
                col = col + 1
                traceStr = traceStr + vbCrLf
                'traceStr.Insert(traceStr.Length, vbCrLf)
            End If
        Next index

        'MessageBox.Show(traceStr, "Char Data")

        convert2BinArr = True
        Return convert2BinArr
    End Function
    Function showChar() As Integer

        Dim readStr As String
        Dim readChar As Integer
        Dim pathStr As String
        Dim HeaderSize, CodeTableCount, CharSize, CodeTableSize, BinSize, CodeTableOffset As Integer
        Dim CodeDataOffset, index As UInteger
        Dim row, col, x, y As Integer
        Dim ret As Boolean

        pathStr = ""
        pathStr = FilePath.Text.ToString()

        ret = OpenRead(pathStr)
        If ret = False Then
            Return False
        End If

        readStr = ShowCharText.Text
        If IsNumeric(readStr) Then
            readChar = Convert.ToInt32(readStr)
        ElseIf IsHex(readStr) Then
            readChar = HexStr2Int(readStr, 2)
        Else
            MessageBox.Show("Please keyin Numeric in [ShowChar] box", "[ShowChar] Error")
            Return False
        End If


        'Get Char address
        HeaderSize = binHeader.FontHeaderSize
        CodeTableCount = binHeader.CodeTableCount
        BinSize = binHeader.FileTotalSize

        CharSize = binHeader.PixHeight * binHeader.WidthByte
        CodeTableSize = CodeTableCount * 4
        CodeTableOffset = HeaderSize + (readChar * 4)

        If readChar > CodeTableCount Then
            MessageBox.Show("[ShowChar] over range", "[ShowChar] Error")
        End If

        'Get Char data
        Dim CharDataArray(CharSize) As Byte
        'Get offset value
        Using fs As FileStream = File.Open(pathStr, FileMode.Open)
            'ReadByteFlow(fs, CodeTableOffset)
            'CodeDataOffset = Read4ByteInv(fs)
            CodeDataOffset = GetCharOffset(fs, pathStr, CodeTableOffset, False)
            fs.Close()
        End Using
        If (CodeDataOffset = 0) Then
            ' 無此字
            Dim gT As Graphics = PictureBox1.CreateGraphics()
            ' 將畫布清為白色
            gT.Clear(ColorTranslator.FromOle(QBColor(15)))
            Return False
        End If
        ''check offset value, if 0xFFFF FFFF
        'If CodeDataOffset >= 4294967295 Then
        '    MessageBox.Show("No this char!", "Code Table Err")
        '    Return
        'End If
        ''check offset value, if over BinSize
        'If CodeDataOffset >= BinSize Then
        '    MessageBox.Show("Code Table Err! Over file size.", "Code Table Err")
        '    Return
        'End If

        Using fs As FileStream = File.Open(pathStr, FileMode.Open)
            ReadByteFlow(fs, CodeDataOffset)
            ReadByteArray(fs, CharSize, CharDataArray)
            fs.Close()
        End Using

        ' convert to binary for pixel, integer to binary
        'Dim PixelDataArray(binHeader.PixHeight - 1, binHeader.PixWidth - 1) As Byte
        Dim width_B As Integer = (binHeader.WidthByte * 8)
        Dim PixelDataArray(binHeader.PixHeight - 1, (width_B) - 1) As Byte
        Dim binaryStr As String
        Dim i, stop1 As Integer
        Dim traceStr As String = "", traceStr2 As String
        x = 0
        y = 0
        row = 0
        col = 0
        For index = 0 To CharSize - 1 Step 1

            binaryStr = Convert.ToString(CharDataArray(index), 2)
            If binaryStr.Length < 8 Then
                'add '0'
                For i = 1 To 8 - binaryStr.Length
                    binaryStr = "0" + binaryStr
                Next
            End If
            'If (x + 8) < binHeader.PixWidth Then
            If (x + 8) < width_B Then
                stop1 = 7
            Else
                'stop1 = binHeader.PixWidth - x - 1
                stop1 = width_B - x - 1
            End If

            For i = 0 To stop1 Step 1
                traceStr2 = Mid(binaryStr, i + 1, 1)
                PixelDataArray(col, x) = traceStr2
                If traceStr.Length > 0 Then
                    traceStr = traceStr + traceStr2
                    'If traceStr2 = "0" Then
                    '    traceStr = traceStr + "0"
                    'ElseIf traceStr2 = "1" Then
                    '    traceStr = traceStr + "@"
                    'End If
                Else
                    traceStr = traceStr2
                End If
                x = x + 1
            Next
            'If x >= binHeader.PixWidth Then
            If x >= width_B Then
                x = 0
                col = col + 1
                traceStr = traceStr + vbCrLf
                'traceStr.Insert(traceStr.Length, vbCrLf)
            End If
        Next index
        'MessageBox.Show(traceStr, "Char Data")

        'Show to PictureBox1
        Dim g As Graphics = PictureBox1.CreateGraphics()
        Dim LPen As New System.Drawing.Pen(System.Drawing.Color.Black)
        Dim b As New SolidBrush(Color.Black)

        ' 將畫布 g 清為白色
        g.Clear(ColorTranslator.FromOle(QBColor(15)))

        'LPen.DashStyle = Drawing2D.DashStyle.Dot
        'g.DrawLine(LPen, 1, 25, 2, 25)
        'g.DrawString("XXX", New Font("Arial", 44, FontStyle.Bold), Brushes.Blue, 20, 20)
        For y = 0 To binHeader.PixHeight - 1
            'For x = 0 To binHeader.PixWidth - 1
            For x = 0 To width_B - 1
                If PixelDataArray(y, x) = 1 Then
                    'g.FillRectangle(Brushes.Red, x, y, 1, 1)
                    g.FillRectangle(Brushes.Black, x, y, 1, 1)
                End If
            Next
        Next
        Return True
    End Function
    ' 顯示指定字
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        showChar()
    End Sub
    ' 存檔
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim pathStr As String
        Dim ret As Boolean = False
        'Dim myImageCodecInfo As ImageCodecInfo

        'open, and read header
        pathStr = ""
        pathStr = FilePath.Text.ToString()
        ret = OpenRead(pathStr)
        If ret = False Then
            Return
        End If
        pathStr = String.Format("{0}{1}PNG", GetFileRootFromPath(pathStr), GetFileNameFromPathWithoutExt(pathStr))
        'myImageCodecInfo = GetEncoderInfo(ImageFormat.Bmp)
        PictureBox1.Image.Save(pathStr, ImageFormat.Png)
    End Sub
    ' 上個字
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim readStr As String
        Dim readChar As UShort
        Dim pathStr As String
        Dim CodeTableOffset As Integer
        Dim CodeDataOffset As UInteger
        Dim ret As Boolean

        readStr = ShowCharText.Text
        If IsNumeric(readStr) Then
            readChar = Convert.ToInt32(readStr)
        ElseIf IsHex(readStr) Then
            readChar = HexStr2Int(readStr, 2)
        Else
            MessageBox.Show("Please keyin Numeric in [ShowChar] box", "[ShowChar] Error")
            Return
        End If

        readChar = readChar - 1
        If readChar < binHeader.FirstChar Then
            readChar = binHeader.FirstChar
        End If

        pathStr = ""
        pathStr = FilePath.Text.ToString()

        ret = OpenRead(pathStr)
        If ret = False Then
            Dim gT As Graphics = PictureBox1.CreateGraphics()
            ' 將畫布清為白色
            gT.Clear(ColorTranslator.FromOle(QBColor(15)))
            Return
        End If

        'Get Char address
        While True
            CodeTableOffset = binHeader.FontHeaderSize + (readChar * 4)
            Using fs As FileStream = File.Open(pathStr, FileMode.Open)
                CodeDataOffset = GetCharOffset(fs, pathStr, CodeTableOffset, False)
                fs.Close()
            End Using

            If (CodeDataOffset <> 0) Then
                ' find char, break while
                ShowCharText.Text = readChar
                Exit While
            Else
                If readChar = binHeader.FirstChar Then
                    ' not find, first char, break while
                    Exit While
                Else
                    ' not find, check next char
                    readChar = readChar - 1
                End If
            End If
        End While
        showChar()
    End Sub
    ' 下個字
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim readStr As String
        Dim readChar As UShort
        Dim pathStr As String
        Dim CodeTableOffset As Integer
        Dim CodeDataOffset As UInteger
        Dim ret As Boolean

        readStr = ShowCharText.Text
        If IsNumeric(readStr) Then
            readChar = Convert.ToInt32(readStr)
        ElseIf IsHex(readStr) Then
            readChar = HexStr2Int(readStr, 2)
        Else
            MessageBox.Show("Please keyin Numeric in [ShowChar] box", "[ShowChar] Error")
            Return
        End If
        readChar = readChar + 1
        If readChar > 255 Then
            readChar = 255
        End If

        pathStr = ""
        pathStr = FilePath.Text.ToString()

        ret = OpenRead(pathStr)
        If ret = False Then
            Dim gT As Graphics = PictureBox1.CreateGraphics()
            ' 將畫布清為白色
            gT.Clear(ColorTranslator.FromOle(QBColor(15)))
            Return
        End If

        'Get Char address
        While True
            CodeTableOffset = binHeader.FontHeaderSize + (readChar * 4)
            Using fs As FileStream = File.Open(pathStr, FileMode.Open)
                CodeDataOffset = GetCharOffset(fs, pathStr, CodeTableOffset, False)
                fs.Close()
            End Using

            If (CodeDataOffset <> 0) Then
                ' find char, break while
                ShowCharText.Text = readChar
                Exit While
            Else
                If readChar = 255 Then
                    ' not find, last char, break while
                    Exit While
                Else
                    ' not find, check next char
                    readChar = readChar + 1
                End If
            End If
        End While
        showChar()
    End Sub
    ' 清除超過寬度的位元
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim readStr As String
        Dim readChar As UShort
        Dim PixW As UShort = binHeader.PixWidth
        Dim PixH As UShort = binHeader.PixHeight
        Dim Width_B As UShort = binHeader.WidthByte
        Dim PixW_All As UShort = (binHeader.WidthByte * 8)
        Dim CodeTableCount As UShort = binHeader.CodeTableCount
        Dim BinSize As Integer = binHeader.FileTotalSize
        Dim CharSize As Integer
        Dim CodeTableSize As Integer
        'Dim CodeTableOffset As Integer
        Dim CodeDataOffset As UInteger
        Dim charOffset As UInteger
        Dim ret As Boolean

        readStr = ShowCharText.Text
        If IsNumeric(readStr) Then
            readChar = Convert.ToInt32(readStr)
        ElseIf IsHex(readStr) Then
            readChar = HexStr2Int(readStr, 2)
        Else
            MessageBox.Show("Please keyin Numeric in [ShowChar] box", "[ShowChar] Error")
            Return
        End If

        Dim pathStr As String

        pathStr = ""
        pathStr = FilePath.Text.ToString()
        ' check pathStr
        ret = OpenRead(pathStr)
        If ret = False Then
            Return
        End If

        'check readChar
        CodeTableSize = CodeTableCount * 4
        If readChar > CodeTableCount Then
            MessageBox.Show("[ShowChar] over range", "[ShowChar] Error")
            Return
        End If

        Dim gT As Graphics = PictureBox1.CreateGraphics()
        ' 將畫布清為白色
        gT.Clear(ColorTranslator.FromOle(QBColor(15)))

        Dim codeTable(binHeader.CodeTableCount) As UInteger

        Using fs As FileStream = File.Open(pathStr, FileMode.Open)
            'Get offset value
            ReadByteFlow(fs, binHeader.FontHeaderSize)
            'read code table
            For i As Integer = 0 To binHeader.CodeTableCount - 1 Step 1
                charOffset = Read4ByteInv(fs)
                codeTable(i) = charOffset
            Next
            fs.Close()
        End Using

        CharSize = PixH * Width_B
        Dim CharDataArray(CharSize) As Byte
        Dim LastByteIndex As UShort = Width_B - 1
        Dim clrBits As Integer = PixW_All - PixW
        Dim andBits As Byte = (1 << clrBits) - 1
        'Dim revBits As Integer = (8 - clrBits)
        Dim charTemp, charT2 As Byte
        Dim isChange As Boolean = False
        For readChar = binHeader.FirstChar To 255 Step 1

            'Get offset value
            'CodeTableOffset = binHeader.FontHeaderSize + (readChar * 4)

            'Using fs As FileStream = File.Open(pathStr, FileMode.Open)
            'CodeDataOffset = GetCharOffset(fs, pathStr, CodeTableOffset, False)
            'End Using
            CodeDataOffset = codeTable(readChar)
            isChange = False

            'check offset value, not 0xFFFF FFFF, and not 0, and < FileTotalSize
            If (CodeDataOffset < 4294967295 And CodeDataOffset > 0 And CodeDataOffset < binHeader.FileTotalSize) Then

                'Get Char data
                Using fs As FileStream = File.Open(pathStr, FileMode.Open)
                    ReadByteFlow(fs, CodeDataOffset)
                    ReadByteArray(fs, CharSize, CharDataArray)
                    fs.Close()
                End Using

                For i As Integer = LastByteIndex To CharSize - 1 Step Width_B
                    charTemp = CharDataArray(i)
                    charT2 = charTemp And andBits
                    If charT2 <> 0 Then
                        charTemp = charTemp >> clrBits
                        charTemp = charTemp << clrBits
                        CharDataArray(i) = charTemp
                        isChange = True
                    End If
                Next
                If isChange = True Then
                    Using fs As FileStream = File.Open(pathStr, FileMode.Open)
                        ReadByteFlow(fs, CodeDataOffset)
                        WriteByteArray(fs, CharSize, CharDataArray)
                        fs.Close()
                    End Using
                End If

            End If
        Next

        showChar()
    End Sub
    Sub New()

        ' 設計工具需要此呼叫。
        InitializeComponent()

        ' 在 InitializeComponent() 呼叫之後加入所有初始設定。
        CreateRegistry()
        ReadRegistry()
        FilePath.Text = sFilePath

    End Sub

End Class
