/**
 * Live image preview helper for file upload inputs.
 *
 * Usage in Razor views:
 *   <input type="file" name="thumbnailFile" accept="image/*"
 *          onchange="previewImg(this, 'my-preview-id')" />
 *   <div id="my-preview-id"></div>
 */
function previewImg(input, containerId) {
    var container = document.getElementById(containerId);
    if (!container) return;
    container.innerHTML = '';
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var img = document.createElement('img');
            img.src = e.target.result;
            img.style.cssText = 'max-height:120px;max-width:100%;border-radius:6px;';
            img.className = 'border p-1 mt-1';
            container.appendChild(img);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
