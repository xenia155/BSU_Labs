% Copyright

implement map inherits formWindow
    open core, vpiDomains

clauses
    display(Parent) = Form :-
        Form = new(Parent),
        Form:show().

clauses
    new(Parent) :-
        formWindow::new(Parent),
        generatedInitialize().

predicates
    onPaint : window::paintResponder.
clauses
    onPaint(Source, _Rectangle, _GDI) :-
        W = Source:getVPIWindow(),
        draw::drawThem(W).

predicates
    onPushButtonClick : button::clickResponder.
clauses
    onPushButtonClick(_Source) = _ :-
        InputText1 = edit_ctl:getText(),
        InputText2 = edit1_ctl:getText(),
        draw::findDistance(InputText1, InputText2, edit2_ctl),
        fail.
    onPushButtonClick(_Source) = button::defaultAction().

% This code is maintained automatically, do not update it manually.
facts
    edit_ctl : editControl.
    edit1_ctl : editControl.
    pushButton_ctl : button.
    edit2_ctl : editControl.

predicates
    generatedInitialize : ().
clauses
    generatedInitialize() :-
        setText("map"),
        setRect(vpiDomains::rct(50, 40, 534, 360)),
        setDecoration(titlebar([frameDecoration::maximizeButton, frameDecoration::minimizeButton, frameDecoration::closeButton])),
        setBorder(frameDecoration::sizeBorder),
        setState([wsf_ClipSiblings, wsf_ClipChildren]),
        menuSet(noMenu),
        setPaintResponder(onPaint),
        edit_ctl := editControl::new(This),
        edit_ctl:setText("Edit"),
        edit_ctl:setPosition(208, 40),
        edit1_ctl := editControl::new(This),
        edit1_ctl:setText("Edit"),
        edit1_ctl:setPosition(208, 68),
        StaticText_ctl = textControl::new(This),
        StaticText_ctl:setText("1st city:"),
        StaticText_ctl:setPosition(172, 40),
        StaticText_ctl:setWidth(32),
        StaticText1_ctl = textControl::new(This),
        StaticText1_ctl:setText("2nd city:"),
        StaticText1_ctl:setPosition(172, 68),
        StaticText1_ctl:setWidth(32),
        pushButton_ctl := button::new(This),
        pushButton_ctl:setText("Find"),
        pushButton_ctl:setPosition(204, 94),
        pushButton_ctl:setClickResponder(onPushButtonClick),
        StaticText2_ctl = textControl::new(This),
        StaticText2_ctl:setText("Result:"),
        StaticText2_ctl:setPosition(268, 96),
        StaticText2_ctl:setWidth(28),
        edit2_ctl := editControl::new(This),
        edit2_ctl:setPosition(300, 96),
        edit2_ctl:setReadOnly(true).
% end of automatic code

end implement map
