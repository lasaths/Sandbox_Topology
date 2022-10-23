Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry

Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types

Public Class TopologyLine
    Inherits GH_Component
    ''' <summary>
    ''' Initializes a new instance of the PolygonEdgeTopology class.
    ''' </summary>
    Public Sub New()
        MyBase.New("Line Topology", "Line Topo", _
     "Analyses the topology of a network consisting of lines", _
     "Sandbox", "Topology")
    End Sub

    ''' <summary>
    ''' Registers all the input parameters for this component.
    ''' </summary>
    Protected Overrides Sub RegisterInputParams(ByVal pManager As GH_Component.GH_InputParamManager)
        pManager.AddLineParameter("List of lines", "L", "Network of lines", GH_ParamAccess.list)
        pManager.AddNumberParameter("Tolerance", "T", "Tolerance value", GH_ParamAccess.item, 0.001)
    End Sub

    ''' <summary>
    ''' Registers all the output parameters for this component.
    ''' </summary>
    Protected Overrides Sub RegisterOutputParams(ByVal pManager As GH_Component.GH_OutputParamManager)
        pManager.AddPointParameter("List of points", "P", "Ordered list of unique points", GH_ParamAccess.list)
        pManager.AddIntegerParameter("Line-Point structure", "LP", "For each line lists both end points indices", GH_ParamAccess.tree)
        pManager.AddIntegerParameter("Point-Point structure", "PP", "For each point list all point indices connected to it", GH_ParamAccess.tree)
        pManager.AddLineParameter("Point-Line structure", "PL", "For each point list all lines connected to it", GH_ParamAccess.tree)
    End Sub

    ''' <summary>
    ''' This is the method that actually does the work.
    ''' </summary>
    ''' <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

        '1. Declare placeholder variables and assign initial invalid data.
        '   This way, if the input parameters fail to supply valid data, we know when to abort.
        Dim _L As New List(Of Line)
        Dim _T As Double = 0

        '2. Retrieve input data.
        If (Not DA.GetDataList(0, _L)) Then Return
        If (Not DA.GetData(1, _T)) Then Return

        '3. Abort on invalid inputs.
        If (Not _L.Count > 0) Then Return
        If (Not _T > 0) Then Return

        '4. Do something useful.
        Dim _polyList As New List(Of Polyline)

        '4.1. check inputs
        For Each _line As Line In _L
            Dim _poly As Polyline = New Polyline(New Point3d() {_line.From(), _line.To()})
            _polyList.Add(_poly)
        Next

        '4.1. get topology
        Dim _ptList As List(Of PointTopological) = getPointTopo(_polyList, _T)
        Dim _lineList As List(Of PLineTopological) = getPLineTopo(_polyList, _ptList, _T)
        Call setPointPLineTopo(_lineList, _ptList)

        ' 4.2: return results
        Dim _PValues As New List(Of Point3d)
        For Each _ptTopo As PointTopological In _ptList
            _PValues.Add(_ptTopo.Point)
        Next

        Dim _LPValues As New Grasshopper.DataTree(Of Int32)
        For Each _lineTopo As PLineTopological In _lineList
            Dim _path As New GH_Path(_LPValues.BranchCount)
            _LPValues.Add(_lineTopo.PointIndices.Item(0), _path)
            _LPValues.Add(_lineTopo.PointIndices.Item(1), _path)
        Next

        Dim _PPValues As New Grasshopper.DataTree(Of Int32)
        For Each _ptTopo As PointTopological In _ptList
            Dim _path As New GH_Path(_PPValues.BranchCount)
            For Each _lineTopo As PLineTopological In _ptTopo.PLines
                If _ptTopo.Index = _lineTopo.PointIndices.Item(0) Then
                    _PPValues.Add(_lineTopo.PointIndices.Item(1), _path)
                ElseIf _ptTopo.Index = _lineTopo.PointIndices.Item(1) Then
                    _PPValues.Add(_lineTopo.PointIndices.Item(0), _path)
                End If
            Next
        Next

        Dim _PLValues As New Grasshopper.DataTree(Of Line)
        For Each _ptTopo As PointTopological In _ptList
            Dim _path As New GH_Path(_PLValues.BranchCount)
            For Each _lineTopo As PLineTopological In _ptTopo.PLines
                _PLValues.Add(_L.Item(_lineTopo.Index), _path)
            Next
        Next

        DA.SetDataList(0, _PValues)
        DA.SetDataTree(1, _LPValues)
        DA.SetDataTree(2, _PPValues)
        DA.SetDataTree(3, _PLValues)


    End Sub

    Public Overrides ReadOnly Property Exposure As Grasshopper.Kernel.GH_Exposure
        Get
            Return GH_Exposure.primary
        End Get
    End Property

    ''' <summary>
    ''' Provides an Icon for every component that will be visible in the User Interface.
    ''' Icons need to be 24x24 pixels.
    ''' </summary>
    Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
        Get
            'You can add image files to your project resources and access them like this:
            Return My.Resources.TopologyLine
        End Get
    End Property

    ''' <summary>
    ''' Gets the unique ID for this component. Do not change this ID after release.
    ''' </summary>
    Public Overrides ReadOnly Property ComponentGuid() As Guid
        Get
            Return New Guid("{a09956f1-a616-4896-a242-eab3fc506087}")
        End Get
    End Property
End Class