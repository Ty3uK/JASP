$('.clickable').click(function(){
	$.blockUI({
		message: '<img src="'+$(this).attr('src')+'" />',
		css: {width: 1, height: 1, top: '0%', left: '0%', cursor: 'pointer'},
		overlayCSS: {cursor: 'pointer'}
	}); 
	$('.blockOverlay').click($.unblockUI);
	$('.blockMsg').click($.unblockUI); 
});