    function onSelection() {        
            const selection = window.getSelection();      
            if (!selection.rangeCount) return;
     
            const parent = selection.anchorNode?.parentElement;
      
            if (!parent) return;
     
            if (parent.tagName === 'STRONG') {     
                parent.textContent = '**' + parent.textContent + '**';   
                parent.style.color = '#ffa500';     
            }
        } 
        function getContent() { 
            return document.querySelector('.container').innerHTML;
          }