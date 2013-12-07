
/* Disqus Comment System */

//$(function() {
//    var dsq = document.createElement('script');
//    dsq.type = 'text/javascript';
//    dsq.async = true;
//    dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
//    (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
//});




/* SyntaxHighlighter */

$(function () {

    SyntaxHighlighter.autoloader(
        'js jscript javascript            shBrushJScript.js',
        'c-sharp, csharp	                shBrushCSharp.js',
        'ps, powershell	                shBrushPowerShell.js',
        'xml, xhtml, xslt, html, xhtml	shBrushXml.js',
        'sql                              shBrushSql.js',
        'bash, shell	                    shBrushBash.js',
        'plain, text	                    shBrushPlain.js'
    );

    var elementsToBeHighlighted = SyntaxHighlighter.findElements().length,
        highlightedElements = 0;

    SyntaxHighlighter.complete = function(callback) {

        (function recountHighlightedElements() {
            setTimeout(function() {
                highlightedElements = $('.syntaxhighlighter');
                if (highlightedElements.length < elementsToBeHighlighted) {
                    recountHighlightedElements();
                } else {
                    callback();
                }
            }, 200);
        })();
    };

    SyntaxHighlighter.complete(function() {
        $(".syntaxhighlighter").parent().addClass("sh-padding-right");
    });
                
    SyntaxHighlighter.all();
});
