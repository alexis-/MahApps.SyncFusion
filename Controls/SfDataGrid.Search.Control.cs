#region Copyright Syncfusion Inc. 2001 - 2016

// Copyright Syncfusion Inc. 2001 - 2016. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 

#endregion



namespace MahApps.SyncFusion.Controls
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Threading;

  using Syncfusion.UI.Xaml.Grid;
  using Syncfusion.UI.Xaml.Utility;

  /// <summary>class which helps to Search the Text in the DataGrid.</summary>
  [TemplatePart(Name = "PART_FindNext", Type = typeof(Button))]
  [TemplatePart(Name = "PART_FindPrevious", Type = typeof(Button))]
  [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
  [TemplatePart(Name = "PART_ApplyFiltering", Type = typeof(CheckBox))]
  [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
  [TemplatePart(Name = "PART_CaseSensitiveSearch", Type = typeof(CheckBox))]
  public class SfDataGridSearchControl : Control, IDisposable
  {
    #region Fields

    /// <summary>The data grid property</summary>
    public static readonly DependencyProperty DataGridProperty =
      DependencyProperty.Register(
        "DataGrid", typeof(SfDataGrid), typeof(SfDataGridSearchControl),
        new PropertyMetadata(new PropertyChangedCallback(OnDataGridPropertyChanged)));

    /// <summary>The search command property</summary>
    public static readonly DependencyProperty SearchCommandProperty =
      DependencyProperty.Register(
        "SearchCommand", typeof(ICommand), typeof(SfDataGridSearchControl),
        new PropertyMetadata(null));

    /// <summary>The close command property</summary>
    public static readonly DependencyProperty CloseCommandProperty =
      DependencyProperty.Register(
        "CloseCommand", typeof(ICommand), typeof(SfDataGridSearchControl),
        new PropertyMetadata(null));
    private bool _isSettingVisibility = false;

    #endregion



    #region Constructors

    /// <summary>Initializes static members of the <see cref="SfDataGridSearchControl" /> class.</summary>
    static SfDataGridSearchControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(
        typeof(SfDataGridSearchControl),
        new FrameworkPropertyMetadata(typeof(SfDataGridSearchControl)));
    }

    /// <summary>Initializes a new instance of the <see cref="SfDataGridSearchControl" /> class.</summary>
    public SfDataGridSearchControl()
    {
      SearchCommand = new BaseCommand(OnSearchExecute);
    }

    /// <summary>Initializes a new instance of the <see cref="SfDataGridSearchControl" /> class.</summary>
    /// <param name="datagrid">The datagrid.</param>
    public SfDataGridSearchControl(SfDataGrid datagrid) : this()
    {
      DataGrid = datagrid;
    }

    #endregion



    #region Properties

    /// <summary>Gets or sets the DataGrid for the corresponding search operation.</summary>
    public SfDataGrid DataGrid
    {
      get { return (SfDataGrid)GetValue(DataGridProperty); }
      set { SetValue(DataGridProperty, value); }
    }

    /// <summary>Gets or sets the search command.</summary>
    public ICommand SearchCommand
    {
      get { return (ICommand)GetValue(SearchCommandProperty); }
      set { SetValue(SearchCommandProperty, value); }
    }
    
    /// <summary>Gets or sets the close command.</summary>
    public ICommand CloseCommand
    {
      get { return (ICommand)GetValue(CloseCommandProperty); }
      set { SetValue(CloseCommandProperty, value); }
    }


    /// <summary>Gets or sets the adorner layer.</summary>
    internal AdornerDecorator AdornerLayer { get; set; }

    /// <summary>Gets or sets the apply filter CheckBox.</summary>
    internal CheckBox ApplyFilterCheckBox { get; set; }

    /// <summary>Gets or sets the case sensitive search CheckBox.</summary>
    internal CheckBox CaseSensitiveSearchCheckBox { get; set; }

    /// <summary>Gets or sets the close button.</summary>
    internal Button CloseButton { get; set; }

    /// <summary>Gets or sets the find next button.</summary>
    internal Button FindNextButton { get; set; }

    /// <summary>Gets or sets the find previous button.</summary>
    internal Button FindPreviousButton { get; set; }

    /// <summary>Gets or sets the search text box.</summary>
    internal TextBox SearchTextBox { get; set; }

    #endregion



    #region Methods

    /// <summary>
    ///   Performs application-defined tasks associated with freeing, releasing, or resetting
    ///   unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      UnWireEvents();
      DataGrid = null;
    }

    /// <summary>
    ///   When overridden in a derived class, is invoked whenever application code or internal
    ///   processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      FindNextButton = GetTemplateChild("PART_FindNext") as Button;
      FindPreviousButton = GetTemplateChild("PART_FindPrevious") as Button;
      CloseButton = GetTemplateChild("PART_Close") as Button;
      ApplyFilterCheckBox = GetTemplateChild("PART_ApplyFiltering") as CheckBox;
      SearchTextBox = GetTemplateChild("PART_TextBox") as TextBox;
      CaseSensitiveSearchCheckBox = GetTemplateChild("PART_CaseSensitiveSearch") as CheckBox;
      AdornerLayer = GetTemplateChild("PART_AdornerLayer") as AdornerDecorator;

      WireEvents();
    }

    /// <summary>Method to open Search Control.</summary>
    /// <param name="visible">adjust control depending on visiblity.</param>
    /// <param name="setVisibility">if set to <c>true</c> updates control's visibility.</param>
    internal void UpdateSearchControlVisiblity(bool visible, bool setVisibility = false)
    {
      if (_isSettingVisibility)
        return;

      _isSettingVisibility = setVisibility;

      if (visible)
      {
        if (setVisibility)
          Visibility = Visibility.Visible;

        Dispatcher.BeginInvoke(
          (Action)delegate { SearchTextBox.Focus(); }, DispatcherPriority.Render);
      }
      else
      {
        if (setVisibility)
          Visibility = Visibility.Collapsed;

        SearchTextBox.Clear();
        DataGrid.SearchHelper.ClearSearch();
        DataGrid.Focus();
      }

      _isSettingVisibility = false;
    }

    private static void OnDataGridPropertyChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      var dataGrid = dependencyPropertyChangedEventArgs.NewValue as SfDataGrid;

      if (dataGrid == null)
        throw new InvalidOperationException("Invalid DataGrid object");

      dataGrid.SearchHelper.SearchBrush = dataGrid.FindResource("AccentColorBrush3") as Brush;
      dataGrid.SearchHelper.SearchHighlightBrush =
        dataGrid.FindResource("AccentColorBrush") as Brush;
    }

    /// <summary>Method to wire the required events.</summary>
    private void WireEvents()
    {
      FindNextButton.Click += OnFindNextButtonClick;
      FindPreviousButton.Click += OnFindPreviousButtonClick;
      CloseButton.Click += OnCloseButtonClick;
      SearchTextBox.TextChanged += OnTextChanged;
      SearchTextBox.KeyDown += SearchTextBoxOnKeyDown;
      ApplyFilterCheckBox.Click += OnApplyFilterCheckBoxClick;
      CaseSensitiveSearchCheckBox.Click += OnCaseSensitiveSearchCheckBoxClick;
      AdornerLayer.KeyDown += OnAdornerLayerKeyDown;
      IsVisibleChanged += OnIsVisibleChanged;
    }

    private void SearchTextBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
    {
      if (keyEventArgs.Key == Key.Enter)
        Search();
    }

    /// <summary>Event handler to handle AdornerLayer key down.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
    private void OnAdornerLayerKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnClosing();
    }

    /// <summary>Event handler to handle CaseSensitive search check box click.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void OnCaseSensitiveSearchCheckBoxClick(object sender, RoutedEventArgs e)
    {
      var grid = GetDataGrid();
      grid.SearchHelper.AllowCaseSensitiveSearch = (bool)CaseSensitiveSearchCheckBox.IsChecked;
      Search();
    }

    /// <summary>Event handler to handle when text value is changed in SearchTextBox.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
      var grid = GetDataGrid();
      grid.SearchHelper.Search(SearchTextBox.Text);
      Search();
    }

    /// <summary>Event handler to handle when clicking on FindNext button.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void OnFindNextButtonClick(object sender, RoutedEventArgs e)
    {
      Search();
    }

    /// <summary>Event handler to handle when clicking on FindPrevious button.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void OnFindPreviousButtonClick(object sender, RoutedEventArgs e)
    {
      var grid = GetDataGrid();
      grid.SearchHelper.FindPrevious(SearchTextBox.Text);
    }

    /// <summary>Called when search command is executed.</summary>
    /// <param name="o">Should be null.</param>
    private void OnSearchExecute(object o)
    {
      var grid = GetDataGrid();
      grid.SearchHelper.FindNext(SearchTextBox.Text);
    }

    /// <summary>Shortcut to search next.</summary>
    private void Search()
    {
      OnSearchExecute(null);
    }

    /// <summary>Event handler to handle when clicking on Close button.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
      OnClosing();
    }

    /// <summary>Event handler to handle ApplyFilter check box click.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void OnApplyFilterCheckBoxClick(object sender, RoutedEventArgs e)
    {
      var grid = GetDataGrid();
      grid.SearchHelper.AllowFiltering = (bool)ApplyFilterCheckBox.IsChecked;
    }

    private void OnClosing()
    {
      if (CloseCommand != null)
      {
        if (CloseCommand.CanExecute(null))
          CloseCommand.Execute(null);
      }
      else
        UpdateSearchControlVisiblity(false, true);
    }

    /// <summary>Method to return the DataGrid which Selected in the ComboBox.</summary>
    /// <returns></returns>
    private SfDataGrid GetDataGrid()
    {
      return DataGrid;
    }

    private void OnIsVisibleChanged(
      object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      UpdateSearchControlVisiblity((bool)dependencyPropertyChangedEventArgs.NewValue);
    }

    /// <summary>Method to UnWire the wired events.</summary>
    private void UnWireEvents()
    {
      FindNextButton.Click -= OnFindNextButtonClick;
      FindPreviousButton.Click -= OnFindPreviousButtonClick;
      CloseButton.Click -= OnCloseButtonClick;
      SearchTextBox.TextChanged -= OnTextChanged;
      ApplyFilterCheckBox.Click -= OnApplyFilterCheckBoxClick;
      IsVisibleChanged -= OnIsVisibleChanged;
    }

    #endregion
  }
}