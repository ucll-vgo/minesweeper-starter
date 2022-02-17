require 'fileutils'
require 'pathname'
require 'find'


def is_root?(filename)
    %r{// ROOT} =~ IO.readlines(filename).first
end

def find_root_asciidocs_files
    Find.find('.').select do |filename|
        filename.end_with? '.asciidoc' and is_root? filename
    end
end

def compile_asciidoc
    puts "Compiling asciidocs..."
    roots = find_root_asciidocs_files
    `asciidoctor -R docs -D dist #{roots.join(' ')}`
end

def compile_graphviz
    root = Pathname.new 'docs'
    dist = Pathname.new 'dist'

    Find.find('docs') do |entry|
        if entry.end_with? '.gv'
            path = Pathname.new entry
            relative_path = path.relative_path_from root
            from_path = root + relative_path
            to_path = dist + relative_path

            puts "#{from_path} -> #{to_path}"
            output = `dot -Tsvg #{from_path.expand_path} -o #{to_path.expand_path.sub_ext('.svg')}`.strip
            puts output if output.size > 0
        end
    end
end

desc 'Removes dist directory'
task :clean do
    FileUtils.rm_rf 'dist'
end

namespace :build do
    desc 'Compiles asciidoc'
    task :asciidoc do
        compile_asciidoc
        compile_graphviz
    end

    desc 'Compiles graphviz'
    task :graphviz do
        compile_graphviz
    end

    desc 'Compiles everything'
    task :all => [ 'build:asciidoc', 'build:graphviz' ]
end

desc 'Uploads dist to server'
task :upload do
    Dir.chdir 'dist' do
        `ssh -p 22345 -l upload leone.ucll.be rm -rf /home/frederic/courses/vgo/volume/*`
        puts `scp -P 22345 -r * upload@leone.ucll.be:/home/frederic/courses/vgo/volume`
    end
end

desc 'Equivalent to build:all'
task :default => [ "build:all" ]

desc 'Does the whole shebang including uploading'
task :full => [ :clean, 'build:all', :upload ]