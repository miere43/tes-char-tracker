{
  New script template, only shows processed records
  Assigning any nonzero value to Result will terminate script
}
unit CELLED;

var CellsList: TStringList;
var SkyrimESM: IwbFile;

// Called before processing
// You can remove it if script doesn't require initialization code
function Initialize: integer;
begin
  Result := 0;

  CellsList := TStringList.Create;
  CellsList.Add('EditorID;Cell FormID;X;Y;Z');
  SkyrimESM := FileByIndex(0);
  // BuildRef(SkyrimESM);
end;

// called for every record selected in xEdit
function Process(e: IInterface): integer;
var
  xtelElement: IwbElement;
  cellEDID: string;
  cellHexFormID: string;
  xtelX: string;
  xtelY: string;
  xtelZ: string;
  owningCell: IwbContainer;
  cellObjects: IwbGroupRecord;
  i: integer;
  doorRefID: variant;
  doorRef: IwbMainRecord;
  doorRefCell: IwbContainer;
  doorRefCellFormID: integer;
  cell: IwbElement;
begin
  Result := 0;

  if (Signature(e) <> 'CELL') then begin
    Exit;
  end;

  cellHexFormID := IntToHex(FormID(e), 8);
  // AddMessage('Form ID: ' + cellHexFormID);

  // Get Persistent objects
  cellObjects := ElementByIndex(ChildGroup(e), 0);
  for i:=0 to ElementCount(cellObjects)-1 do begin
    xtelElement := ElementBySignature(ElementByIndex(cellObjects, i), 'XTEL');

    if Assigned(xtelElement) then begin
      doorRefID := GetElementNativeValues(xtelElement, 'Door');
      doorRef := RecordByFormID(SkyrimESM, doorRefID, false);
      doorRefCell := GetContainer(GetContainer(GetContainer(doorRef)));

      doorRefCellFormID := FormID(ElementByIndex(doorRefCell, 0));

      // 3444 = 0x0000D74
      if doorRefCellFormID <> 3444 then begin
        // AddMessage('Door dont lead to Skyrim: ' + IntToHex(doorRefID, 8));
        Exit;
      end;

      xtelX := GetElementEditValues(xtelElement, 'Position/Rotation\Position\X');
      xtelY := GetElementEditValues(xtelElement, 'Position/Rotation\Position\Y');
      xtelZ := GetElementEditValues(xtelElement, 'Position/Rotation\Position\Z');

      CellsList.Add(EditorID(e) + ';' + IntToHex(FixedFormID(e), 8) + ';' + xtelX + ';' + xtelY + ';' + xtelZ);

      //if not (Assigned(doorRefCell)) or (Signature(doorRefCell) <> 'CELL') then begin
      //  Exit;
      //end;
      
      //AddMessage('FullPath: ' + FullPath(GetContainer(doorRef)));
      // AddMessage('LinksTo: ' + FullPath(ReferencedByCount(doorRef, 2)));
    end;
  end;

  // AddMessage(IntToStr(ElementCount(cellObjects)));
end;

// Called after processing
// You can remove it if script doesn't require finalization code
function Finalize: integer;
var
  filename: string;
begin
  Result := 0;
  filename := ProgramPath + 'output.csv';
  AddMessage('Saving to ' + filename);
  CellsList.SaveToFile(filename);

  CellsList.Free;
end;

end.
