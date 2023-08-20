<template>
  <div>
    <ckeditor></ckeditor>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { component as ckeditor } from '@ckeditor/ckeditor5-vue';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
    
const editor = ClassicEditor
// const ckeditor = CKEditor.component

const editorData = ref('<p>Content of the editor.</p>');


</script>

  
  

  
<style>

    
</style>